using System;
using System.Collections.Generic;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace NetWork.Codec
{
    internal class MessageDecode : ByteToMessageDecoder
    {
        private readonly IMessageProcessor m_MessageProcessor;

        public MessageDecode(IMessageProcessor messageProcessor)
        {
            m_MessageProcessor = messageProcessor;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            using (var stream = new MemoryStream(input.ReadableBytes))
            {
                input.GetBytes(input.ReaderIndex, stream, input.ReadableBytes);
                using (var reader = new BinaryReader(stream))
                {
                    stream.Position = 0;
                    var messageId = reader.ReadUInt32();
                    var message = m_MessageProcessor.CreateMessage(messageId);
                    
                }
            }
            
        }
    }
}