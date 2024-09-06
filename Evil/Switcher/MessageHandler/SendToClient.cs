using Evil.Switcher;

namespace Proto
{
    public partial class SendToClient
    {
        public override async Task Dispatch()
        {
            var linkerSession = Linker.I.Sessions.GetSession(clientSessionId);
            if (linkerSession is null)
            {
                // TODO 与ClientRspResponse一样处理
            }
            else
            {
                await linkerSession.SendAsync(new ServerMsgBox
                {
                    messageId = messageId,
                    data = data,
                });
            }
        }
    }
}