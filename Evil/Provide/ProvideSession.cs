﻿using System.Collections.Concurrent;
using DotNetty.Transport.Channels;
using Evil.Util;
using NetWork;
using Proto;
using ProtoBuf;

namespace Evil.Provide
{
    public class ProvideSession : Session
    {
        private readonly string m_ProviderUrl;
            
        private readonly ConcurrentDictionary<long, ClientContext> m_ClientContexts = new();

        public string ProviderUrl => m_ProviderUrl;
        
        public ProvideSession(IChannelHandlerContext context) : base(context)
        {
            m_ProviderUrl = RemoteAddress();
        }

        public ProvideSession AddClient(ClientContext ctx)
        {
            m_ClientContexts[ctx.ClientSessionId] = ctx;
            SendAsync(new BindClient{clientSessionId = ctx.ClientSessionId});
            return this;
        }

        public async Task SendToClientAsync(long clientSessionId, Message msg)
        {
            MessageHelper.OnSendMsg(clientSessionId, msg, "client");
            // 考虑优化，现在是在逻辑线程同步编码
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, msg);

            await SendAsync(new SendToClient
            {
                clientSessionId = clientSessionId,
                messageId = msg.MessageId,
                data = stream.GetBuffer()[..(int)stream.Length],
            });
        }

        public override void OnClose()
        {
            base.OnClose();
            foreach (var pair in m_ClientContexts)
            {
                try
                {
                    pair.Value.OnClientBroken();
                }
                catch (Exception e)
                {
                    Log.I.Error($"client session {pair.Key}", e);
                }
            }
            m_ClientContexts.Clear();
        }

        internal void ClientBroken(long clientSessionId)
        {
            var ctx = GetClientContext(clientSessionId);
            if (ctx is not null)
            {
                ctx.OnClientBroken();
            }
        }
        
        public ClientContext? GetClientContext(long clientSessionId)
        {
            return m_ClientContexts.TryGetValue(clientSessionId, out var ctx) ? ctx : null;
        }
    }
}