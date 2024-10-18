using System;
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
        [JsonIgnore]
        public virtual uint MessageId { get; } = 0;
        [JsonIgnore]
        public virtual int MaxSize { get; } = 1024 * 1024;
        [JsonIgnore]
        public ushort InnerPvid { get; set; }
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
            session.Send(this);
        }
        
        /// <summary>
        /// 在网络线程中执行，保证顺序
        /// </summary>
        /// <returns></returns>
        public virtual void Dispatch()
        {
            try
            {
                Session.Config.Dispatcher!.Dispatch(this);   
            } catch (Exception e)
            {
                Log.I.Error($"dispatch msg {this}", e);
            }
        }

        /// <summary>
        /// 异步执行，不保证顺序
        /// </summary>
        public virtual bool Process()
        {
            return false;
        }

        public virtual void Decode(BinaryReader reader)
        {
        }
        
        public virtual void Encode(BinaryWriter writer)
        {
        }
    }
}