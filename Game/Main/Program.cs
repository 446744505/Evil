﻿
using System.Threading.Tasks;
using Edb;
using Evil.Event;
using Evil.Provide;
using Evil.Util;
using Game.NetWork;

namespace Game
{
    public static partial class Program
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
                
                var provide = Net.I.Provide = new Provide(new GameProvideFactory());
                await provide.Start(CmdLine.I.Etcd);
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