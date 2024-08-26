using Evil.Util;
using NetWork;
using Proto;

namespace Game.NetWork
{
    public partial class Net : Singleton<Net>
    {
        private readonly MessageRegister m_MessageRegister = new();
        public IMessageRegister MessageRegister => m_MessageRegister;
    }
}