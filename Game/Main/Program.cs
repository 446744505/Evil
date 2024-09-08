
using System.Threading.Tasks;
using Edb;
using Evil.Event;
using Evil.Provide;
using Evil.Util;
using Game.NetWork;
using Game.Test;

namespace Game
{
    public static partial class Program
    {
        public static async Task Main1(string[] args)
        {
            // Log.I.UnobservedTaskException();
            // CmdLine.Init(args);
            // Etcd.I.Init(CmdLine.I.Etcd);
            // Event.Start();
            //
            // Stopper? stopper = null;
            // try
            // {
            //     await Edb.Edb.I.Start(new Config(), XTable.Tables.All);
            //     
            //     var provide = Net.I.Provide = new Provide(new GameProvideFactory());
            //     await provide.Start();
            //     Log.I.Info("server started");
            //
            //     stopper = new Stopper().BindAndWait();
            //     
            //     Etcd.I.Dispose();
            //     provide.Dispose();
            //     await Edb.Edb.I.DisposeAsync();
            // }
            // finally
            // {
            //     stopper?.SignalWeakUp();
            //     Log.I.Info("server stop");
            // }

            await new TransactionTest().TestWhenCommit();
            await Edb.Edb.I.DisposeAsync();
        }
    }
}