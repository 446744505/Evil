using Evil.Util;

namespace Evil
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.I.UnobservedTaskException();
            MessageIgnore.Init();
            CmdLine.Init(args);
            Etcd.I.Init(CmdLine.I.Etcd);
            
            Stopper? stopper = null;
            try
            {
                // start
                switch (CmdLine.I.Node)
                {
                    case "switcher":
                        Switcher.Switcher.I.Start(args);       
                        break;
                    default:
                        throw new NotSupportedException("Not Supported Node");
                }
                
                stopper = new Stopper().BindAndWait();
                
                // stop
                Etcd.I.Dispose();
                switch (CmdLine.I.Node)
                {
                    case "switcher": 
                        Switcher.Switcher.I.Stop();
                        break;
                }
            }
            finally
            {
                stopper?.SignalWeakUp();   
                Log.I.Info("Switcher Stop");
            }
        }

    }
}