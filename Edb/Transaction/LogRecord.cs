using System.Collections.Concurrent;
using Evil.Util;

namespace Edb
{
    public class LogRecord<TKey, TValue> where TKey : notnull where TValue : class
    {
        private readonly TTable<TKey, TValue> m_Table;
        private readonly Dictionary<TKey, LogR<TKey, TValue>> m_Changed = new();
        private readonly Listenable m_Seed;
        private bool? m_HasListener;
        
        private static readonly ConcurrentDictionary<Type, Listenable> m_Cache = new();
        internal LogRecord(TTable<TKey, TValue> table)
        {
            var o = table.NewValue();
            m_Table = table;
            m_Seed = o == null
                ? Listenable.DefaultListenable
                : m_Cache.GetOrAdd(o.GetType(), _ => Listenable.Create(o));
        }
        
        private LogR<TKey, TValue>? GetOrCreateLogR(TRecord<TKey, TValue> r, TransactionCtx ctx)
        {
            if (m_HasListener == null)
            {
                m_HasListener = m_Table.HasListener;
                ctx.Current!.RecordLogNotifyTTable(m_Table);
            }
            
            return m_HasListener.Value ? m_Changed.ComputeIfAbsent(r.Key, 
                _ => new LogR<TKey, TValue>(r, m_Seed.Copy())) : null;
        }

        internal async Task LogNotify(ListenerMap listenerMap, TransactionCtx ctx)
        {
            foreach (var pair in m_Changed)
            {
                var k = pair.Key;
                var lr = pair.Value;
                await lr.m_Listenable.LogNotify<TKey, TValue>(k, lr.m_Record.Value!, lr.RecordState, listenerMap, ctx);
                m_Changed.Clear();
                m_HasListener = null;
            }
        }

        internal void OnChanged(TRecord<TKey,TValue> record, LogNotify ln, TransactionCtx ctx)
        {
            var lr = GetOrCreateLogR(record, ctx);
            if (lr != null)
            {
                ln.Pop();
                lr.m_Listenable.SetChanged<TKey, TValue>(ln);
            }
        }

        internal void OnChanged(TRecord<TKey,TValue> record, bool cc, TRecord<TKey,TValue>.State ss, TransactionCtx ctx)
        {
            var lr = GetOrCreateLogR(record, ctx);
            if (lr != null && lr.m_Ss == null)
            {
                lr.m_Cc = cc;
                lr.m_Ss = ss;
            }
        }

        private class LogR<TK, TV> where TK : notnull where TV : class
        {
            public readonly TRecord<TK, TV> m_Record;
            public readonly Listenable m_Listenable;
            public bool m_Cc;
            public TRecord<TK, TV>.State? m_Ss;

            public LogR(TRecord<TK, TV> record, Listenable listenable)
            {
                m_Record = record;
                m_Listenable = listenable;
            }
            
            public RecordState RecordState {
                get
                {
                    var st = m_Record.Stat;
                    if (null == m_Ss)
                        return RecordState.Changed;
                    if (m_Cc && TRecord<TK, TV>.State.Add == m_Ss && TRecord<TK, TV>.State.Remove == st)
                        return RecordState.None;
                    if (!m_Cc && TRecord<TK, TV>.State.Add == m_Ss && TRecord<TK, TV>.State.Remove == st)
                        return RecordState.Removed;
                    if (TRecord<TK, TV>.State.Add == m_Ss && TRecord<TK, TV>.State.Add == st)
                        return RecordState.Added;
                    if (TRecord<TK, TV>.State.InDbGet == m_Ss && TRecord<TK, TV>.State.InDbRemove == st)
                        return RecordState.Removed;
                    if (TRecord<TK, TV>.State.InDbGet == m_Ss && TRecord<TK, TV>.State.InDbAdd == st)
                        return RecordState.Added;
                    if (TRecord<TK, TV>.State.InDbRemove == m_Ss && TRecord<TK, TV>.State.InDbAdd == st)
                        return RecordState.Added;
                    if (m_Cc && TRecord<TK, TV>.State.InDbRemove == m_Ss && TRecord<TK, TV>.State.InDbRemove == st)
                        return RecordState.Removed;
                    if (!m_Cc && TRecord<TK, TV>.State.InDbRemove == m_Ss && TRecord<TK, TV>.State.InDbRemove == st)
                        return RecordState.None;
                    if (m_Cc && TRecord<TK, TV>.State.Remove == m_Ss && TRecord<TK, TV>.State.Remove == st)
                        return RecordState.None;
                    if (m_Cc && TRecord<TK, TV>.State.Remove == m_Ss && TRecord<TK, TV>.State.Add == st)
                        return RecordState.Added;
                    if (!m_Cc && TRecord<TK, TV>.State.InDbAdd == m_Ss && TRecord<TK, TV>.State.InDbAdd == st)
                        return RecordState.Added;
                    if (!m_Cc && TRecord<TK, TV>.State.InDbAdd == m_Ss && TRecord<TK, TV>.State.InDbRemove == st)
                        return RecordState.Removed;
                    throw new Exception($"LogRecord Error! isCreateCache = {m_Cc}, SavedState = {m_Ss}, State = " + st);
                }
            }
        }
    }

    public enum RecordState
    {
        Added,
        Removed,
        Changed,
        None
    }
}