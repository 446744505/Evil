using Evil.Switcher;

namespace Proto
{
    public partial class BindClient
    {
        public override Task<bool> Process()
        {
            var linkerSession = Linker.I.Sessions.GetSession(clientSessionId);
            if (linkerSession is not null)
            {
                var providerSession = (ProviderSession)Session;
                linkerSession.BindProvide(providerSession.Pvid);
            }

            return TrueTask;
        }
    }
}