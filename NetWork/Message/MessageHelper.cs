using System;
using System.Collections.Generic;
using Evil.Util;

namespace NetWork
{
    public class MessageHelper
    {
        private static readonly HashSet<uint> m_IgnoreMsgIds = new();
        public static bool IsDebug => Log.I.IsDebug;

        static MessageHelper()
        {
            m_IgnoreMsgIds.Add(MessageId.RpcResponse);
        }

        public static void IgnoreMsg(params Type[] types)
        {
            foreach (var type in types)
            {
                m_IgnoreMsgIds.Add(MessageIdGenerator.CalMessageId(type));
            }
        }

        private static bool IsSkip(uint msgId)
        {
            if (!IsDebug)
                return true;
            return m_IgnoreMsgIds.Contains(msgId);
        }
        
        public static void OnReceiveMsg(Message msg, string from = "")
        {
            if (IsSkip(msg.MessageId))
                return;
            Log.I.Debug($"receive {from} msg {msg}");
        }
        
        public static void OnReceiveMsg(Session session, Message msg, string from = "")
        {
            if (IsSkip(msg.MessageId))
                return;
            Log.I.Debug($"receive {from} msg {msg}, session {session}");
        }

        public static void OnReceiveMsg(long sessionId, Message msg, string from = "")
        {
            if (IsSkip(msg.MessageId))
                return;
            Log.I.Debug($"receive {from} msg {msg}, sessionId {sessionId}");
        }

        public static void OnSendMsg(Message msg, string to = "")
        {
            if (IsSkip(msg.MessageId))
                return;
            Log.I.Debug($"send {to} msg {msg}");
        }
        
        public static void OnSendMsg(Session session, Message msg, string to = "")
        {
            if (IsSkip(msg.MessageId))
                return;
            Log.I.Debug($"send {to} msg {msg}, session {session}");
        }
        
        public static void OnSendMsg(long sessionId, Message msg, string to = "")
        {
            if (IsSkip(msg.MessageId))
                return;
            Log.I.Debug($"send {to} msg {msg}, sessionId {sessionId}");
        }
    }
}