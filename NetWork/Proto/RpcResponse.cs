using System.IO;
using System.Threading.Tasks;
using ProtoBuf;

namespace NetWork.Proto
{
    [ProtoContract]
    public class RpcResponse : Message
    {
        public override uint MessageId => NetWork.MessageId.RpcResponse;
        public override int MaxSize => 0;

        [ProtoMember(1)]
        public long RequestId { get; set; }

        [ProtoMember(2)] 
        public byte[] Data { get; set; } = null!;

        public override Task<bool> Process()
        {
            var cb = Session.Transport.RpcMgr().RemovePending(RequestId);
            if (cb == null)
            {
                return FalseTask;
            }

            using var stream = new MemoryStream(Data);
            cb.Invoke(stream);
            return TrueTask;
        }
    }
}