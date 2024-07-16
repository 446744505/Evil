using System.Runtime.Loader;

namespace Evil.Util
{
    public sealed class Stopper
    {
        private volatile bool m_IsStop;
        private readonly AutoResetEvent m_Event = new(false);
        private AutoResetEvent? m_SignalEvent;

        public Stopper BindSignal()
        {
            AssemblyLoadContext.Default.Unloading += WeakUp;
            return this;
        }

        public Stopper BindCancelKey()
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                WeakUp(null!);
            };
            return this;
        }

        private void WeakUp(AssemblyLoadContext _)
        {
            if (m_IsStop)
            {
                return;
            }
            
            Log.I.Info("signal stopper weak up");
            WeakUp();
            // 等其他线程结束
            m_SignalEvent = new(false);
            m_SignalEvent.WaitOne();
        }

        private void WeakUp()
        {
            m_Event.Set();
            m_IsStop = true;
        }
        
        public void SignalWeakUp()
        {
            m_SignalEvent?.Set();
        }

        public Stopper Wait()
        {
            Log.I.Info("stopper start wait");
            m_Event.WaitOne();
            Log.I.Info("stopper finish");
            return this;
        }
    }
}