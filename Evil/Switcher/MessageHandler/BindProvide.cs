
using Evil.Switcher;

namespace Proto
{
    public partial class BindProvide
    {
        public override void Dispatch()
        {
            var providerSession = (ProviderSession)Session;
            providerSession.Process(this);
        }
    }
}