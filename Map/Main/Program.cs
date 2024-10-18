
using Edb;
using Evil.Event;
using Evil.Provide;
using Evil.Util;

namespace Map
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.I.UnobservedTaskException();
            CmdLine.Init(args);
            Etcd.I.Init(CmdLine.I.Etcd);
            Event.Start();
            
            Stopper? stopper = null;
            try
            {
                await Edb.Edb.I.Start(new Config(), XTable.Tables.All);
                
                var provide = new Provide(new MapProvideFactory());
                await provide.Start();
                Log.I.Info("map started");

                stopper = new Stopper().BindAndWait();
                
                Etcd.I.Dispose();
                provide.Dispose();
                await Edb.Edb.I.Dispose();
            }
            finally
            {
                stopper?.SignalWeakUp();
                Log.I.Info("map stop");
            }
        }
    }
}