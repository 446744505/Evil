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

        internal void NotifyChanged(string fullVarName, object key, object value)
        {
            Notify(ChangeKind.ChangedAll, fullVarName, key, value, null);
        }
        
        internal void NotifyChanged(string fullVarName, object key, object value, INote? note)
        {
            Notify(ChangeKind.ChangedNote, fullVarName, key, value, note);
        }
        
        internal void NotifyRemoved(string fullVarName, object key, object value)
        {
            Notify(ChangeKind.Removed, fullVarName, key, value, null);
        }

        private void Notify(ChangeKind kind, string fullVarName, object key, object value, INote? note)
        {
            var localCopy = m_ListenersCopy;
            var listeners = localCopy[fullVarName];
            if (listeners == null)
                return;
            foreach (var l in listeners)
            {
                var transaction = Transaction.Current!;
                var spBefore = transaction.CurrentSavepointId;
                var spBeforeAccess = spBefore > 0 ? transaction.GetSavepoint(spBefore)!.Access : 0;
                try
                {
                    switch (kind)
                    {
                        case ChangeKind.ChangedAll:
                            l.OnChanged(key, value);
                            break;
                        case ChangeKind.ChangedNote:
                            l.OnChanged(key, value, fullVarName, note);
                            break;
                        case ChangeKind.Removed:
                            l.OnRemoved(key, value);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Log.I.Error($"doChanged key={key} name={fullVarName}", e);
                    var spAfter = transaction.CurrentSavepointId;
                    if (spBefore == 0)
                    {
                        if (spAfter > 0)
                            transaction._rollback(spBefore + 1);
                    }
                    else
                    {
                        if (spAfter < spBefore)
                            throw new Exception("spAfter < spBefore");
                        if (transaction.GetSavepoint(spBefore)!.IsAccessSince(spBeforeAccess))
                            transaction._rollback(spBefore);
                        else if (spAfter > spBefore)
                            transaction._rollback(spBefore + 1);
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