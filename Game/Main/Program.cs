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
                
                var netConfig = new ProvideConnectorTransportConfig(1);
                netConfig.Port = 10001;
                // 设置消息处理器为带edb的事务处理
                netConfig.Dispatcher = new ProcedureHelper.MessageDispatcher();
                var provide = new Provide(netConfig, Net.I.MessageRegister);
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