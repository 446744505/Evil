using System.IO;
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

        public override void Process()
        {
            var func = RpcMgr.I.RemovePending(RequestId);
            if (func == null)
            {
                return;
            }

            using var stream = new MemoryStream(Data);
            func.Invoke(stream);
        }
    }
}