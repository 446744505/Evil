using System.Collections.Concurrent;
using Evil.Util;

namespace Evil.Switcher
{
    internal class ProviderSessions
    {
        private readonly ConcurrentDictionary<ushort, ProviderSession> m_Sessions = new();

        internal ProviderSession? GetSession(ushort pvid)
        {
            return m_Sessions.TryGetValue(pvid, out var session) ? session : null;
        }
        
        internal async Task Bind(ProviderSession session)
        {
            var pvid = session.Pvid;
            if (m_Sessions.TryGetValue(pvid, out var old))
            {
                await session.CloseAsync();
                Log.I.Error($"bind provide {pvid} failed, old session {old} is still alive");
                return;
            }

            m_Sessions[pvid] = session;
        }
        internal void UnBind(ProviderSession session)
        {
            var pvid = session.Pvid;
            if (m_Sessions.TryGetValue(pvid, out var old))
            {
                if (session == old)
                {
                    m_Sessions.Remove(pvid, out _);
                }
                else
                {
                    Log.I.Error($"try unbind a error provide,old {old} new {session}");
                }
            }
            else
            {
                Log.I.Error($"unbind provide {pvid} failed, session is not exist");
            }
        }
    }
}