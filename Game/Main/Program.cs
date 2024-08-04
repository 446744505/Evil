﻿using System.Threading.Tasks;
using Edb;
using Evil.Util;
using Game.NetWork;
using Game.Test;
using NetWork.Transport;

namespace Game
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var test = new NotifyTest();
            await test.TestUpdate();
            // Stopper? stopper = null;
            // try
            // {
            //     await Edb.Edb.I.Start(new Config(), XTable.Tables.All);
            //     
            //     var netConfig = new AcceptorTransportConfig();
            //     // 设置消息处理器为带edb的事务处理
            //     netConfig.Dispatcher = new ProcedureHelper.MessageDispatcher();
            //     netConfig.NetWorkFactory = new GameNetWorkFactory();
            //     var acceptor = new AcceptorTransport(netConfig);
            //     acceptor.Start();
            //     Log.I.Info("server started");
            //     
            //     stopper = new Stopper()
            //         .BindSignal()
            //         .BindCancelKey()
            //         .Wait();
            //     
            //     acceptor.Dispose();
            //     await Edb.Edb.I.DisposeAsync();
            // }
            // finally
            // {
            //     stopper?.SignalWeakUp();
            //     Log.I.Info("server stop");
            // }
        }
    }
}