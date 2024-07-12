using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProtoBuf;

namespace NetWork.Codec
{
    internal class MessageEncode : MessageToByteEncoder<Message>
    {
        protected override void Encode(IChannelHandlerContext context, Message message, IByteBuffer output)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(message.MessageId);
                    message.Encode(writer);
                    Serializer.Serialize(stream, message);
                    output.WriteBytes(stream.GetBuffer(), 0, (int)stream.Length);
                }
            }
        }
    }
}