using System.Threading.Tasks;
using Edb;
using Evil.Util;
using Game.NetWork;
using NetWork;
using NetWork.Transport;

namespace Game
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Stopper? stopper = null;
            try
            {
                await Edb.Edb.I.Start(new Config(), XTable.Tables.All);
                // 设置消息处理器为edb事务处理
                Message.Dispatcher = new ProcedureHelper.MessageDispatcher();
                
                var netConfig = new AcceptorTransportConfig();
                netConfig.NetWorkFactory = new GameNetWorkFactory();
                var acceptor = new AcceptorTransport(netConfig);
                await acceptor.Start();
                Log.I.Info("server started");
                
                stopper = new Stopper()
                    .BindSignal()
                    .BindCancelKey()
                    .Wait();
                
                acceptor.Dispose();
                await Edb.Edb.I.DisposeAsync();
            }
            finally
            {
                stopper?.SignalWeakUp();
            }
        }
    }
}