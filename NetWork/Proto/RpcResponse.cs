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
            var completionSource = RpcMgr.I.RemovePending(RequestId);
            if (completionSource == null)
            {
                return;
            }
            using (var stream = new MemoryStream(Data))
            {
                var message = Serializer.Deserialize(completionSource.Task.GetType(), stream) as Message;
                completionSource.TrySetResult(message!);
            }
        }
    }
}