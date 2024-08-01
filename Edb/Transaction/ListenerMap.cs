using Evil.Util;

namespace Edb
{
    public class ListenerMap
    {
        private readonly Dictionary<string, HashSet<IListener>?> m_Listeners = new();
        private readonly LockAsync m_Lock = new();
        private volatile Dictionary<string, HashSet<IListener>?> m_ListenersCopy = new();
        
        private void SetListenersCopy()
        {
            m_ListenersCopy = new(m_Listeners.Count);
            foreach (var pair in m_Listeners)
            {
                m_ListenersCopy[pair.Key] = new HashSet<IListener>(pair.Value!);
            }
        }
        
        internal Action Add(string name, IListener listener)
        {
            var release = m_Lock.WLock();
            try
            {
                m_Listeners.ComputeIfAbsent(name, _ => new HashSet<IListener>())!.Add(listener);
                SetListenersCopy();
                return () =>
                {
                    var r = m_Lock.WLock();
                    try
                    {
                        m_Listeners.ComputeIfPresent(name, (k, v) =>
                        {
                            if (v!.Remove(listener))
                                SetListenersCopy();
                            return v.Count == 0 ? default : v;
                        });
                    } finally
                    {
                        m_Lock.WUnlock(r);
                    }
                };
            } finally
            {
                m_Lock.WUnlock(release);
            }
        }

        internal bool HasListener()
        {
            var localCopy = m_ListenersCopy;
            return localCopy.Count > 0;
        }
        
        internal bool HasListener(string fullVarName)
        {
            var localCopy = m_ListenersCopy;
            return localCopy.ContainsKey(fullVarName);
        }

        internal async Task NotifyChanged(string fullVarName, object key, object value)
        {
            await Notify(ChangeKind.ChangedAll, fullVarName, key, value, null);
        }
        
        internal async Task NotifyChanged(string fullVarName, object key, object value, INote? note)
        {
            await Notify(ChangeKind.ChangedNote, fullVarName, key, value, note);
        }
        
        internal async Task NotifyRemoved(string fullVarName, object key, object value)
        {
            await Notify(ChangeKind.Removed, fullVarName, key, value, null);
        }

        private async Task Notify(ChangeKind kind, string fullVarName, object key, object value, INote? note)
        {
            var localCopy = m_ListenersCopy;
            if (!localCopy.TryGetValue(fullVarName, out var listeners))
                return;
            
            foreach (var l in listeners!)
            {
                var transaction = Transaction.Current!;
                var spBefore = transaction.CurrentSavepointId;
                var spBeforeAccess = spBefore > 0 ? transaction.GetSavepoint(spBefore)!.Access : 0;
                try
                {
                    switch (kind)
                    {
                        case ChangeKind.ChangedAll:
                            await l.OnChanged(key, value);
                            break;
                        case ChangeKind.ChangedNote:
                            await l.OnChanged(key, value, fullVarName, note);
                            break;
                        case ChangeKind.Removed:
                            await l.OnRemoved(key, value);
                            break;
                    }
                }
                catch (Exception e)
                {
                    /*
                     * 回调错误处理规则。
                     *     spBefore     spAfter     rollback      desc
                     *     -----------------------------------------------------------
                     * (a) 0            0           NONE          前后都没有保存点
                     * (b) 0            >0          spBefore + 1  开始前没有，回调中创建了保存点。
                     * (c) >0 Any       <spBefore   ERROR         错误
                     * (d) >0 Dirty     -           spBefore      开始前的保存点发生了改变
                     * (e) >0 Clean     >spBefore   spBefore + 1
                     * (f) >0 Clean     =spBefore   NONE          回调没有修改操作，
                     * 
                     */
                    Log.I.Error($"doChanged key={key} name={fullVarName}", e);
                    var spAfter = transaction.CurrentSavepointId;
                    if (spBefore == 0)
                    {
                        if (spAfter > 0) // b
                            transaction._rollback(spBefore + 1);
                        // else a
                    }
                    else
                    {
                        if (spAfter < spBefore) // c
                            throw new Exception("spAfter < spBefore");
                        if (transaction.GetSavepoint(spBefore)!.IsAccessSince(spBeforeAccess)) // d
                            transaction._rollback(spBefore);
                        else if (spAfter > spBefore) // e
                            transaction._rollback(spBefore + 1);
                        // else f
                    }
                }
            }
        }
        
        private enum ChangeKind
        {
            ChangedAll,
            ChangedNote,
            Removed,
        }
    }
}