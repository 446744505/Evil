using System.Collections.Generic;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProtoBuf;

namespace NetWork.Codec
{
    internal class MessageDecode : ByteToMessageDecoder
    {
        /// <summary>
        /// 一个连接用一个BinaryReader就好，减少gc，但是内存占用会稍微高一些
        /// </summary>
        private readonly BinaryReader m_Reader;
        private readonly IMessageProcessor m_MessageProcessor;

        public MessageDecode(IMessageProcessor messageProcessor)
        {
            m_MessageProcessor = messageProcessor;
            m_Reader = new BinaryReader(new MemoryStream());
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var stream = m_Reader.BaseStream;
            // 重用reader
            stream.Position = 0;
            // 读取所有数据到stream
            input.GetBytes(input.ReaderIndex, stream, input.ReadableBytes);
            
            // 重新设置流的位置，开始decode
            stream.Position = 0;
            var messageId = m_Reader.ReadUInt32();
            var message = m_MessageProcessor.CreateMessage(messageId);
            // decode head
            message.Decode(m_Reader);
            // decode body
            var len = input.ReadableBytes - stream.Position;
            Serializer.NonGeneric.Deserialize(message.GetType(), stream, message, null, len);
            // 跳过已经读取的字节
            input.SkipBytes((int)stream.Position);
            output.Add(message);
        }

        public override void ChannelInactive(IChannelHandlerContext ctx)
        {
            base.ChannelInactive(ctx);
            m_Reader.Dispose();
        }
    }
}