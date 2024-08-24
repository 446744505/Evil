using Evil.Util;
using NetWork;
using Proto;

namespace Evil.Switcher
{
    public class Switcher : Singleton<Switcher>
    {
        private MessageRegister m_MessageRegister = new();
        public IMessageRegister MessageRegister => m_MessageRegister;
        public void Start(string[] args)
        {
            CmdLine.Init(args);
            Linker.I.Start();
            Provider.I.Start();
        }
    }
}