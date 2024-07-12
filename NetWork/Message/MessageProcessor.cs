using System;
using System.Collections.Generic;

namespace NetWork
{
    public interface IMessageProcessor
    {
        public void Register<T>(uint msgId, Func<T> action) where T : Message;
        public Message CreateMessage(uint msgId);
    }
    public class MessageProcessor : IMessageProcessor
    {
        private static readonly Dictionary<uint, Func<Message>> Creaters = new();
        
        public void Register<T>(uint msgId, Func<T> action) where T : Message
        {
            Creaters[msgId] = () => action();
        }
        
        public Message CreateMessage(uint msgId)
        {
            if (!Creaters.TryGetValue(msgId, out var action))
            {
                throw new NetWorkException($"msgId:{msgId} not register");
            }
            return action();
        }
    }
}