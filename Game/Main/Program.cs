using System.Collections.Generic;
using System.Threading.Tasks;
using Edb;
using Evil.Event;
using Evil.Provide;
using Evil.Util;
using Game.NetWork;

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

                var dispatcher = new ProcedureHelper.MessageDispatcher();
                var provideCfgs = new List<ProvideConnectorTransportConfig>();
                foreach (var provider in CmdLine.I.Providers)
                {
                    var netConfig = new ProvideConnectorTransportConfig(CmdLine.I.Pvid);
                    netConfig.Host = provider.Host;
                    netConfig.Port = provider.Port;
                    // 设置消息处理器为带edb的事务处理
                    netConfig.Dispatcher = dispatcher;
                    provideCfgs.Add(netConfig);
                }
                var provide = new Provide(provideCfgs, Net.I.MessageRegister);
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