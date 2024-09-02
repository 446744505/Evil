using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace NetWork
{
    public interface IMessageProcessor
    {
        public void Register<T>(uint msgId, Func<T> func) where T : Message;
        public Message? CreateMessage(Session session, MessageHeader header, int readSize, BinaryReader reader);
    }
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ushort m_Pvid;
        private readonly Dictionary<uint, Func<Message>> m_Creaters = new();

        public MessageProcessor(ushort pvid)
        {
            m_Pvid = pvid;
        }

        public void Register<T>(uint msgId, Func<T> func) where T : Message
        {
            if (m_Creaters.ContainsKey(msgId))
            {
                throw new NetWorkException($"msgId:{msgId} already register");
            }
            m_Creaters[msgId] = func;
        }
        
        public Message? CreateMessage(Session session, MessageHeader header, int readSize, BinaryReader reader)
        {
            var msgId = header.MessageId;
            if (!m_Creaters.TryGetValue(msgId, out var func))
            {
                return WhenNotType(session, header, readSize, reader);
            }
            
            var msg = func();
            msg.Pvid = header.Pvid;
            msg.Session = session;
            var stream = reader.BaseStream;
            // decode ext head
            msg.Decode(reader);
            // decode body
            var len = readSize - stream.Position;
            Serializer.NonGeneric.Deserialize(msg.GetType(), stream, msg, null, len);
            
            // pvid check
            if (msg.Pvid != m_Pvid)
            {
                throw new NetWorkException($"msgId:{msgId} pvid:{msg.Pvid} != {m_Pvid}");
            }
            
            // 最大传输字节check
            if (msg.MaxSize > 0 && readSize > msg.MaxSize)
            {
                throw new NetWorkException($"msgId:{msgId} readSize:{readSize} > MaxSize:{msg.MaxSize}");
            }
            return msg;
        }

        protected virtual Message? WhenNotType(Session session, MessageHeader header, int readSize, BinaryReader reader)
        {
            throw new NetWorkException($"msgId:{header.MessageId} not register");
        }
    }
}