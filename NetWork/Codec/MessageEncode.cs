using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProtoBuf;

namespace NetWork.Codec
{
    internal class MessageEncode : MessageToByteEncoder<Message>
    {
        /// <summary>
        /// 一个连接用一个BinaryWriter就好，减少gc，但是内存占用会稍微高一些
        /// 可以考虑直接使用IByteBuffer性能更好(但是需要研究如何用IByteBuffer编解码protobuf or protobuf-net)
        /// </summary>
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
            m_Writer.Write(message.InnerPvid);
            // encode ext head
            message.Encode(m_Writer);
            // encode body
            Serializer.Serialize(stream, message);
            output.WriteBytes(stream.GetBuffer()[..(int)stream.Position]);
            if (MessageHelper.IsDebug)
            {
                var session = context.GetAttribute(AttrKey.Session).Get();
                MessageHelper.OnSendMsg(session, message);
            }
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            m_Writer.Dispose();
            base.ChannelInactive(context);
        }
    }
}