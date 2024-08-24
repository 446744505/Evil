using System;
using System.Threading.Tasks;
using Client;
using Client.Hero;

namespace Proto
{
    public partial class LoginNtf
    {
        public override Task<bool> Process()
        {
            Program.Executor.ExecuteAsync(() => HeroMgr.I.Test());
            return TrueTask;
        }
    }
}