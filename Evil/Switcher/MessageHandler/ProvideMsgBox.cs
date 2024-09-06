using Evil.Util;
using NetWork;

namespace Proto
{
    public partial class ProvideMsgBox
    {
        public override async Task Dispatch()
        {
            var header = new MessageHeader
            {
                MessageId = messageId,
                Pvid = InnerPvid,
            };
            var reader = new BinaryReader(new MemoryStream(data));
            try
            {
                var msg = Session.Config.MessageProcessor.CreateMessage(Session, header, data.Length, reader);
                msg!.Context = this;
                MessageHelper.OnReceiveMsg(Session, msg, $"provide{fromPvid}");
                await msg.Dispatch();
            }
            catch (Exception e)
            {
                Log.I.Error($"dispatch msg header {header} provider session {Session.Id} error", e);
            }
            finally
            {
                reader.Dispose();
            }
        }
    }
}