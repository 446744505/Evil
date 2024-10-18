using Evil.Util;
using NetWork;
using Proto;

namespace Evil.Switcher
{
    public class ProviderMessageProcessor : MessageProcessor
    {
        protected override Message? WhenNotType(Session session, MessageHeader header, int readSize, BinaryReader reader)
        {
            var toPvid = header.Pvid;
            var fromSession = (ProviderSession)session;
            var fromPvid = fromSession.Pvid;
            var toSession = Provider.I.Sessions.GetSession(toPvid);
            if (toSession is null)
            {
                Log.I.Error($"provide {fromPvid} to provide {toPvid} fail, not found to {toPvid}, header {header}");
                return null;
            }
            
            var stream = reader.BaseStream;
            var len = readSize - stream.Position;
            var data = reader.ReadBytes((int)len);
            var box = new ProvideMsgBox()
            {
                messageId = header.MessageId,
                data = data,
                fromPvid = fromPvid,
            };
            toSession.Send(box);
            return null;
        }
    }
}