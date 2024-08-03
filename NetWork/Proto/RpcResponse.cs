using System.IO;
using System.Threading.Tasks;
using ProtoBuf;

namespace NetWork.Proto
{
    [ProtoContract]
    public class RpcResponse : Message
    {
        public override uint MessageId => NetWork.MessageId.RpcResponse;

        [ProtoMember(1)]
        public long RequestId { get; set; }
        [ProtoMember(2)]
        public byte[] Data { get; set; }

        public override Task<bool> Process()
        {
            var func = RpcMgr.I.RemovePending(RequestId);
            if (func == null)
            {
                return Task.FromResult(false);
            }

            using var stream = new MemoryStream(Data);
            func.Invoke(stream);
            return Task.FromResult(true);
        }
    }
}