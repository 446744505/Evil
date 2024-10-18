using System.IO;

using NetWork;

namespace Proto
{
    public partial class ServerMsgBox
    {
        public override void Dispatch()
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
                msg!.Dispatch();
            }
            finally
            {
                reader.Dispose();
            }
        }
    }
}