using Evil.Switcher;

namespace Proto
{
    public partial class SendToClient
    {
        public override async Task Dispatch()
        {
            var linkerSession = Linker.I.Sessions.GetSession(clientSessionId);
            if (linkerSession == null)
            {
                // TODO 通知其他provide客户端断开
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