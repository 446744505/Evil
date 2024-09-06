using Evil.Provide;

namespace Proto
{
    public partial class ClientBroken
    {
        public override Task<bool> Process()
        {
            var provideSession = (ProvideSession)Session;
            provideSession.ClientBroken(clientSessionId);
            return TrueTask;
        }
    }
}