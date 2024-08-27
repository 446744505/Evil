using System.IO;
using System.Threading.Tasks;
using NetWork;

namespace Proto
{
    public partial class ServerMsgBox
    {
        public override async Task Dispatch()
        {
            var header = new MessageHeader
            {
                MessageId = messageId,
            };
            var reader = new BinaryReader(new MemoryStream(data));
            try
            {
                var msg = Session.Config.MessageProcessor.CreateMessage(Session, header, data.Length, reader);
                MessageHelper.OnReceiveMsg(msg!, "server");
                await msg!.Dispatch();
            }
            finally
            {
                reader.Dispose();
            }
        }
    }
}