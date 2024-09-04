using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Evil.Util;

namespace NetWork
{
    public struct MessageHeader
    {
        public uint MessageId { get; set; }
        public ushort Pvid { get; set; }
        public void Decode(BinaryReader reader)
        {
            MessageId = reader.ReadUInt32();
            Pvid = reader.ReadUInt16();
        }

        public override string ToString()
        {
            return $"{MessageId} {Pvid}";
        }
    }
    public abstract class Message
    {
        protected static readonly Task<bool> FalseTask = Task.FromResult(false);
        protected static readonly Task<bool> TrueTask = Task.FromResult(true);
        
        [JsonIgnore]
        public virtual uint MessageId { get; } = 0;
        [JsonIgnore]
        public virtual int MaxSize { get; } = 1024 * 1024;
        [JsonIgnore]
        public ushort Pvid { get; set; }
        [JsonIgnore]
        public Session Session { get; set; } = null!;
        [JsonIgnore]
        public object? Context { get; set; }

        public void Send(Session? session)
        {
            if (session == null)
            {
                throw new NetWorkException("session is null");
            }
            session.SendAsync(this);
        }
        
        /// <summary>
        /// 在网络线程中执行，保证顺序
        /// </summary>
        /// <returns></returns>
        public virtual Task Dispatch()
        {
            // 因为外面没有await，所以这里要用ContinueWith打印异常
            Session.Config.Dispatcher!.Dispatch(this).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Log.I.Error($"Message.Dispatch {this}", task.Exception);
                }
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步执行，不保证顺序
        /// </summary>
        public virtual Task<bool> Process()
        {
            return FalseTask;
        }

        public virtual void Decode(BinaryReader reader)
        {
        }
        
        public virtual void Encode(BinaryWriter writer)
        {
        }
    }
}