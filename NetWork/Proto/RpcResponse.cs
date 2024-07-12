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
            Log.I.Info(ToString());
        }
    }
}