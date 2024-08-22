using Evil.Util;
using NetWork;
using Proto;

namespace Evil.Switcher
{
    public class Switcher : Singleton<Switcher>
    {
        private MessageRegister m_MessageRegister = new();
        public IMessageRegister MessageRegister => m_MessageRegister;
        public void Start()
        {
            Stopper? stopper = null;
            try
            {
                Linker.I.Start();
                stopper = new Stopper().BindAndWait();
            }
            finally
            {
                stopper?.SignalWeakUp();   
                Log.I.Info("Switcher Stop");
            }
        }
    }
}