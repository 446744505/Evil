using System.IO;
using System.Threading.Tasks;

namespace NetWork
{
    public abstract class Message
    {
        public virtual uint MessageId { get; }
        public ushort Pvid { get; set; }
        public Session Session { get; set; }

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
        public virtual Task Dispatch()
        {
            return Task.Run(Process);
        }

        /// <summary>
        /// 异步执行，不保证顺序
        /// </summary>
        public virtual void Process()
        {
        }

        public virtual void Decode(BinaryReader reader)
        {
            Pvid = reader.ReadUInt16();
        }
        
        public virtual void Encode(BinaryWriter writer)
        {
            writer.Write(Pvid);
        }
    }
}