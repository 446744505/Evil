using System;
using System.Collections.Generic;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace NetWork.Codec
{
    internal class MessageEncode : MessageToMessageEncoder<Message>
    {
        protected override void Encode(IChannelHandlerContext context, Message message, List<object> output)
        {
            Console.WriteLine();
        }
    }
}