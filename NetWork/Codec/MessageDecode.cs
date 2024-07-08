using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace NetWork.Codec
{
    internal class MessageDecode : MessageToMessageDecoder<Message>
    {
        protected override void Decode(IChannelHandlerContext context, Message message, List<object> output)
        {
            
        }
    }
}