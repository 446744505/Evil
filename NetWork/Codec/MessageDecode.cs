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
        /// 可以考虑直接使用IByteBuffer性能更好(但是需要研究如何用IByteBuffer编解码protobuf or protobuf-net)
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
            // 已经被处理粘包的size
            var readAbleSize = input.ReadableBytes;
            var stream = m_Reader.BaseStream;
            // 重用reader
            stream.Position = 0;
            // 读取所有数据到stream
            input.GetBytes(input.ReaderIndex, stream, readAbleSize);
            
            // 重新设置流的位置，开始decode
            stream.Position = 0;
            var header = new MessageHeader();
            header.Decode(m_Reader);
            var message = m_MessageProcessor.CreateMessage(header, readAbleSize, m_Reader);
            if (message != null)
            {
                // 跳过已经读取的字节
                input.SkipBytes((int)stream.Position);
                output.Add(message);   
            }
        }

        public override void ChannelInactive(IChannelHandlerContext ctx)
        {
            base.ChannelInactive(ctx);
            m_Reader.Dispose();
        }
    }
}