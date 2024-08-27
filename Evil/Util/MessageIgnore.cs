using NetWork;
using Proto;

namespace Evil.Util
{
    public class MessageIgnore
    {
        public static void Init()
        {
            MessageHelper.IgnoreMsg(
                typeof(ClientMsgBox), 
                typeof(ServerMsgBox),
                typeof(ClientRspResponse),
                typeof(SendToClient));
        }
    }
}