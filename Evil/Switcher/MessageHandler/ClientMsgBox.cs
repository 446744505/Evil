using Evil.Util;
using NetWork;

namespace Proto
{
    public partial class ClientMsgBox
    {
        public override async Task Dispatch()
        {
            var header = new MessageHeader
            {
                MessageId = messageId,
                Pvid = (ushort)pvid,
            };
            var reader = new BinaryReader(new MemoryStream(data));
            try
            {
                var msg = Session.Config.MessageProcessor.CreateMessage(Session, header, data.Length, reader);
                msg!.Context = this;
                MessageHelper.OnReceiveMsg(clientSessionId, msg, "client");
                await msg.Dispatch();
            }
            catch (Exception e)
            {
                await Session.SendAsync(new ProvideKick(){clientSessionId = clientSessionId, code = ProvideKick.Exception});
                Log.I.Error($"dispatch msg header {header} client session {Session.Id} error", e);
            }
            finally
            {
                reader.Dispose();
            }
        }
    }
}