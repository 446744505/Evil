using System.Collections.Concurrent;
using Evil.Util;
using Proto;

namespace Evil.Switcher
{
    internal class LinkerSessions
    {
        private readonly ConcurrentDictionary<long, LinkerSession> m_Sessions = new();
        
        internal int Count => m_Sessions.Count;

        internal void AddSession(LinkerSession session)
        {
            m_Sessions[session.Id] = session;
            Log.I.Info($"add client {session}");
        }
        
        internal void RemoveSession(LinkerSession session)
        {
            m_Sessions.TryRemove(session.Id, out _);
            Log.I.Info($"remove client {session}");
        }

        internal LinkerSession? GetSession(long sid)
        {
            return m_Sessions.TryGetValue(sid, out var session) ? session : null;
        }

        internal void CheckAlive()
        {
            foreach (var session in m_Sessions.Values)
            {
                try
                {
                    if (!session.IsAlive())
                    {
                        Linker.I.CloseSession(session, SessionError.PingTimeout);
                        Log.I.Warn($"client {session} is time out");
                    }
                } catch (Exception e)
                {
                    Log.I.Error("check link session alive error", e);
                }
            }
        }
    }
}