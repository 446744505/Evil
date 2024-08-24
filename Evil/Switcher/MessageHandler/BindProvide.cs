
using Evil.Switcher;

namespace Proto
{
    public partial class BindProvide
    {
        public override async Task Dispatch()
        {
            var providerSession = (ProviderSession)Session;
            await providerSession.Process(this);
        }
    }
}