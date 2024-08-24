using Evil.Util;
using NetWork;
using Proto;

namespace Evil.Switcher
{
    public class LinkerMessageProcessor : MessageProcessor
    {
        public LinkerMessageProcessor(ushort pvid) : base(pvid)
        {
        }

        protected override Message? WhenNotType(Session session, MessageHeader header, 
            int readSize, BinaryReader reader)
        {
            var pvid = header.Pvid;
            var provider = Provider.I;
            var providerSession = provider.Sessions.GetSession(pvid);
            if (providerSession == null)
            {
                session.SendAsync(new ServerError{pvid = pvid, code = ServerError.NotExistProvide});
                Log.I.Error($"client to provide, no provide {pvid} exist");
                return null;
            }

            var linkerSession = (LinkerSession)session;
            linkerSession.ReceiveUnknown();
            
            // send to provide
            var stream = reader.BaseStream;
            var len = readSize - stream.Position;
            var data = reader.ReadBytes((int)len);
            var box = new ClientMsgBox()
            {
                clientSessionId = linkerSession.Id,
                messageId = header.MessageId,
                pvid = header.Pvid,
                data = data
            };
            providerSession.SendAsync(box);
            
            return null;
        }
    }
}