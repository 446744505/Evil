﻿using Evil.Util;
using NetWork.Transport;
using Proto;

namespace Evil.Switcher
{
    internal class Linker : Singleton<Linker>
    {
        #region 字段

        private readonly LinkerSessions m_Sessions = new();

        #endregion
        
        #region 属性

        internal LinkerSessions Sessions => m_Sessions;
        internal int SessionTimeout { get; private set; }

        #endregion

        internal void Start()
        {
            SessionTimeout = TimeSpan.FromSeconds(CmdLine.I.LinkerSessionTimeout).Milliseconds;

            StartNetWork();
        }

        private void StartNetWork()
        {
            var netConfig = new AcceptorTransportConfig();
            netConfig.Port = CmdLine.I.LinkerPort;
            netConfig.NetWorkFactory = new LinkerNetWorkFactory();
            var acceptor = new AcceptorTransport(netConfig);
            acceptor.Start();
            Log.I.Info("linker started");
        }

        internal bool CanAddSession()
        {
            return m_Sessions.Count < CmdLine.I.MaxSessionCount;
        }

        internal async Task CloseSession(LinkerSession session, int code)
        {
            Log.I.Info($"close client session {session}, code {code}");
            await session.SendAsync(new SessionError { code = code });
            await session.CloseAsync();
        }
    }
}