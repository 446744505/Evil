using NetWork;
using Proto;

namespace Evil
{
    internal class MessageIgnore
    {
        public static void Init()
        {
            MessageHelper.IgnoreMsg(
                typeof(ClientMsgBox), 
                typeof(ServerMsgBox),
                typeof(ClientRpcResponse),
                typeof(SendToClient));
        }
    }
}