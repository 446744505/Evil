using System.Collections.Generic;
using System.Threading.Tasks;
using Edb;
using Evil.Event;
using Evil.Provide;
using Evil.Util;

namespace Game
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.I.UnobservedTaskException();
            CmdLine.Init(args);
            Event.Start();
            
            Stopper? stopper = null;
            try
            {
                await Edb.Edb.I.Start(new Config(), XTable.Tables.All);
                
                var provide = new Provide(new GameProvideFactory());
                provide.Start();
                Log.I.Info("server started");

                stopper = new Stopper().BindAndWait();
                
                provide.Dispose();
                await Edb.Edb.I.DisposeAsync();
            }
            finally
            {
                stopper?.SignalWeakUp();
                Log.I.Info("server stop");
            }
        }
    }
}