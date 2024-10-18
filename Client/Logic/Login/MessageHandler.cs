
using Client;
using Client.Hero;

namespace Proto
{
    public partial class LoginNtf
    {
        public override bool Process()
        {
            Program.Executor.SubmitAsync(() => HeroMgr.I.Test());
            return true;
        }
    }
}