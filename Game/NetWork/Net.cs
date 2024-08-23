using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edb;
using Evil.Provide;
using Evil.Util;
using NetWork;
using Proto;

namespace Game.NetWork
{
    public class Net : Singleton<Net>
    {
        private readonly ConcurrentDictionary<long, GameClientContext> m_Players = new();
        
        private readonly MessageRegister m_MessageRegister = new();
        public IMessageRegister MessageRegister => m_MessageRegister;

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

        public Task<bool> LogicOffline(long playerId)
        {
            Log.I.Info($"player {playerId} logic offline");
            return Procedure.TrueTask;
        }

        public Task<bool> LogicOnline(long playerId)
        {
            Log.I.Info($"player {playerId} logic online");
            return Procedure.TrueTask;
        }

        public async Task<bool> AddPlayer(long playerId, GameClientContext clientContext)
        {
            if (m_Players.TryRemove(playerId, out var old))
            {
                // 顶号
                await clientContext.Session.SendAsync(new ProvideKick
                {
                    clientSessionId = old.ClientSessionId, 
                    code = ProvideKick.HadLogin,
                });
                Log.I.Warn($"player {playerId} had login, kick old clientSessionId {old.ClientSessionId}");
                var suc = await LogicOffline(playerId);
                if (!suc)
                    return false;
            }
        
            m_Players[playerId] = clientContext;
            Log.I.Info($"player {playerId} net online");
            return await LogicOnline(playerId);
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
    }
}