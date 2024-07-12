using System;
using System.Collections.Generic;

namespace NetWork
{
    public interface IMessageProcessor
    {
        public void Register<T>(uint msgId, Func<T> func) where T : Message;
        public Message CreateMessage(uint msgId);
    }
    public class MessageProcessor : IMessageProcessor
    {
        private static readonly Dictionary<uint, Func<Message>> Creaters = new();
        
        public void Register<T>(uint msgId, Func<T> func) where T : Message
        {
            if (Creaters.ContainsKey(msgId))
            {
                throw new NetWorkException($"msgId:{msgId} already register");
            }
            Creaters[msgId] = func;
        }
        
        public Message CreateMessage(uint msgId)
        {
            if (!Creaters.TryGetValue(msgId, out var func))
            {
                throw new NetWorkException($"msgId:{msgId} not register");
            }
            return func();
        }
    }
}