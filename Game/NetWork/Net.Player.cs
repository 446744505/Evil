using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edb;
using Evil.Provide;
using Evil.Util;
using Game.Logic.Login;
using NetWork;
using Proto;

namespace Game.NetWork
{
    public partial class Net
    {
        private readonly ConcurrentDictionary<long, GameClientContext> m_Players = new();
        
        public long PlayerId(Message clientMsg)
        {
            var ctx = (ClientMsgBox)clientMsg.Context!;
            var provideSession = (ProvideSession)clientMsg.Session;
            var clientContext = provideSession.GetClientContext(ctx.clientSessionId);
            if (clientContext == null)
            {
                throw new NullReferenceException($"client {ctx.clientSessionId} ctx is null");
            }
            return ((GameClientContext)clientContext).PlayerId;
        }
        
        public bool AddPlayer(long playerId, GameClientContext clientContext)
        {
            // 如果已经登录，要先踢掉
            if (!KickPlayer(playerId, ProvideKick.HadLogin))
            {
                return false;
            }
            m_Players[playerId] = clientContext;
            Log.I.Info($"player {playerId} net online");
            return (Procedure.Call(new PLogicOnline(playerId))).IsSuccess;
        }

        public bool KickPlayer(long playerId, int reason)
        {
            if (m_Players.TryRemove(playerId, out var old))
            {
                var suc = LogicOffline(playerId);
                if (!suc)
                    return false;
                // 顶号
                old.Session.Send(new ProvideKick
                {
                    clientSessionId = old.ClientSessionId, 
                    code = reason,
                });
                Log.I.Warn($"kick player {playerId} reason {reason} clientSessionId {old.ClientSessionId}");
                return true;
            }

            return true;
        }

        public void RemovePlayer(long playerId)
        {
            if (m_Players.Remove(playerId, out _))
            {
                Log.I.Info($"player {playerId} net offline");
                LogicOffline(playerId);
            }
            else
            {
                Log.I.Error($"net remove player but not found {playerId}");
            }
        }
        public bool LogicOffline(long playerId)
        {
            Log.I.Info($"player {playerId} logic offline");
            return true;
        }
    }
}