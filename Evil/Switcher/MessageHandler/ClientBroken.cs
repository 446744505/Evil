using Evil.Provide;

namespace Proto
{
    public partial class ClientBroken
    {
        public override bool Process()
        {
            var provideSession = (ProvideSession)Session;
            provideSession.ClientBroken(clientSessionId);
            return true;
        }
    }
}