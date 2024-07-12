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
        
        public virtual Task Dispatch()
        {
            return Task.Run(Process);
        }

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