using Evil.Switcher;

namespace Proto
{
    public partial class SendToClient
    {
        public override void Dispatch()
        {
            var linkerSession = Linker.I.Sessions.GetSession(clientSessionId);
            if (linkerSession is not null)
            {
                linkerSession.Send(new ServerMsgBox
                {
                    messageId = messageId,
                    data = data,
                });
            }
        }
    }
}