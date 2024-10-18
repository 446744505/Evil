using Evil.Util;

namespace Edb
{
    public sealed partial class Transaction
    {
        #region Fields

        private readonly Dictionary<LogKey, object> m_Wrappers = new();
        private Dictionary<string, ITable> m_LogNotifyTables = new();
        private readonly Dictionary<string, Dictionary<object, object>> m_CachedTRecords = new();
        private readonly List<Action> m_LastCommitActions = new();

        internal Dictionary<LogKey, object> Wrappers => m_Wrappers;

        #endregion
       

        #region Static

        private static readonly ThreadLocal<Transaction?> ThreadLocal = new();
        private static readonly ThreadLocal<bool> Isolation = new();
        private static readonly LockX IsolationLock = new();
        internal static Transaction? Current => ThreadLocal.Value;
        public static IsolationLevel IsolationLevel => Isolation.Value ? IsolationLevel.Level3 : IsolationLevel.Level2;
        public static bool IsActive => Current != null;

        #endregion
       

        #region Metrics

        private static long TotalCount;
        private static long TotalFalse;
        private static long TotalException;

        #endregion
        
        internal static Transaction Create()
        {
            var self = Current;
            if (self == null)
                ThreadLocal.Value = self = new Transaction();
            return self;
        }
        
        internal static void Destroy()
        {
            ThreadLocal.Value = null;
        }

        internal void RecordLogNotifyTTable<TKey, TValue>(TTable<TKey, TValue> table)
            where TKey : notnull where TValue : class
        {
            m_LogNotifyTables[table.Name] = table;
        }
        
        internal void AddCacheTRecord<TKey, TValue>(TTable<TKey, TValue> table, TRecord<TKey, TValue> r)
            where TKey : notnull where TValue : class
        {
            if (!m_CachedTRecords.TryGetValue(table.Name, out var records))
            {
                records = new();
                m_CachedTRecords[table.Name] = records;
            }
            records[r.Key] = r;
        }
        
        internal void RemoveCacheTRecord<TKey, TValue>(TTable<TKey, TValue> table, TKey key)
            where TKey : notnull where TValue : class
        {
            if (m_CachedTRecords.TryGetValue(table.Name, out var records))
            {
                records.Remove(key);
            }
        }
        
        internal TRecord<TKey, TValue>? GetCacheTRecord<TKey, TValue>(TTable<TKey, TValue> table, TKey key)
            where TKey : notnull where TValue : class
        {
            if (m_CachedTRecords.TryGetValue(table.Name, out var records))
            {
                if (records.TryGetValue(key, out var record))
                {
                    return (TRecord<TKey, TValue>) record;
                }
            }
            return null;
        }
        
        internal void AddLastCommitAction(Action action)
        {
            m_LastCommitActions.Add(action);
        }

        internal void Perform<TP>(ProcedureImpl<TP> p) where TP : IProcedure
        {
            if (p.IsolationLevel == IsolationLevel.Level3)
                IsolationLock.WLock();
            else 
                IsolationLock.RLock();
            try
            {
                Interlocked.Increment(ref TotalCount);
                var flushLock = Edb.I.Tables.FlushLock;
                flushLock.RLock();
                try
                {
                    if (p.Call())
                    {
                        if (_real_commit_(p.Name) > 0)
                        {
                            LogNotify(p);
                        }
                        m_LastCommitActions.ForEach(cb => cb());
                    }
                    else
                    {
                        Interlocked.Increment(ref TotalFalse);
                        _last_rollback_();
                    }
                }
                catch (Exception)
                {
                    _last_rollback_();
                    throw;
                }
                finally
                {
                    m_LastCommitActions.Clear();
                    Finish();
                    flushLock.RUnlock();
                }
            }
            catch (Exception e)
            {
                p.Exception = e;
                p.Success = false;
                Interlocked.Increment(ref TotalException);
                Log.I.Error(e);
                throw;
            }
            finally
            {
                if (p.IsolationLevel == IsolationLevel.Level3)
                    IsolationLock.WUnlock();
                else
                    IsolationLock.RUnlock();
            }
        }

        public static void AddSavepointTask(Action? commitTask, Action? rollbackTask)
        {
            CurrentSavepoint.Add(new TActionLog(commitTask, rollbackTask));
        }
        
        public static void AddSavepointTask(Procedure? commitTask, Procedure? rollbackTask)
        {
            CurrentSavepoint.Add(new TProcedureLog(commitTask, rollbackTask));
        }
        
        public static void SetIsolation(IsolationLevel level)
        {
            switch (level)
            {
                case IsolationLevel.Level2:
                    Isolation.Value = false;
                    break;
                case IsolationLevel.Level3:
                    Isolation.Value = true;
                    break;
            }
        }

        public static void Rollback(int savepoint)
        {
            Current!._rollback(savepoint);
        }

        private void Finish()
        {
            m_Wrappers.Clear();
            foreach (var holder in m_Locks.Values)
            {
                holder.Cleanup();
            }
            m_Locks.Clear();
            m_CachedTRecords.Clear();
        }

        private void LogNotify<TP>(ProcedureImpl<TP> p) where TP : IProcedure
        {
            try
            {
                var maxNestNotify = 255;
                for (var nest = 0; nest < maxNestNotify; nest++)
                {
                    var curLogNotifyTables = m_LogNotifyTables;
                    m_LogNotifyTables = new();
                    foreach (var table in curLogNotifyTables.Values)
                    {
                        table.LogNotify();
                    }

                    if (_real_commit_(p.Name) == 0)
                    {
                        return;
                    }
                }
                Log.I.Fatal($"reach max nest notify proc={p.Name}");
            } catch (Exception e)
            {
                Log.I.Fatal(e);
            }

            _last_rollback_();
            m_LogNotifyTables.Clear();
        }

        internal void _rollback(int savepoint)
        {
            if (savepoint < 1 || savepoint > m_Savepoints.Count)
                throw new XError($"edb: invalid savepoint {savepoint} @ {m_Savepoints.Count}");
            while (m_Savepoints.Count >= savepoint)
            {
                var origin = m_Savepoints[^1];
                m_Savepoints.RemoveAt(m_Savepoints.Count - 1);
                origin.Rollback();
            }
        }
        
        private void _last_rollback_()
        {
            try
            {
                for (var index = m_Savepoints.Count - 1; index >= 0; index--)
                {
                    m_Savepoints[index].Rollback();
                }
                m_Savepoints.Clear();
            } catch (Exception e)
            {
                Log.I.Fatal(e);
                Environment.Exit(270519);
            }
        }

        private int _real_commit_(string pName)
        {
            try
            {
                var count = 0;
                foreach (var sp in m_Savepoints)
                {
                    count += sp.Commit();
                }
                m_Savepoints.Clear();
                return count;
            } catch (Exception e)
            {
                Log.I.Warn(e);
                throw;
            }
        }
        
        private struct TProcedureLog : ILog
        {
            private readonly Procedure? m_Commit;
            private readonly Procedure? m_Rollback;

            public TProcedureLog(Procedure? commit, Procedure? rollback)
            {
                m_Commit = commit;
                m_Rollback = rollback;
            }

            public void Commit()
            {
                m_Commit?.Execute();
            }

            public void Rollback()
            {
                m_Rollback?.Execute();
            }
        }
        
        private struct TActionLog : ILog
        {
            private readonly Action? m_Commit;
            private readonly Action? m_Rollback;

            public TActionLog(Action? commit, Action? rollback)
            {
                m_Commit = commit;
                m_Rollback = rollback;
            }

            public void Commit()
            {
                m_Commit?.Invoke();
            }

            public void Rollback()
            {
                m_Rollback?.Invoke();
            }
        }
    }
    
    public enum IsolationLevel
    {
        Level2,
        Level3
    }
}