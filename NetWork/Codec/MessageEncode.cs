using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProtoBuf;

namespace NetWork.Codec
{
    internal class MessageEncode : MessageToByteEncoder<Message>
    {
        private BinaryWriter m_Writer;
        
        public MessageEncode()
        {
            m_Writer = new BinaryWriter(new MemoryStream());
        }
        
        protected override void Encode(IChannelHandlerContext context, Message message, IByteBuffer output)
        {
            var stream = (MemoryStream)m_Writer.BaseStream;
            // 重用stream
            stream.Position = 0;
            // encode head
            m_Writer.Write(message.MessageId);
            message.Encode(m_Writer);
            // encode body
            Serializer.Serialize(stream, message);
            output.WriteBytes(stream.GetBuffer(), 0, (int)stream.Position);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            m_Writer.Dispose();
        }
    }
}