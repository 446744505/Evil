using System;
using System.IO.Compression;
using System.Threading.Tasks;
using Client.NetWork;
using Logic.Hero.Proto;
using NetWork;
using NetWork.Transport;
using NetWork.Util;

namespace Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Stopper? stopper = null;
            try
            {
                var config = new ConnectorTransportConfig();
                config.SessionFactory = new ClientSessionFactory();
                var connector = new ConnectorTransport(config);
                connector.Start();
                Console.WriteLine("client started");
                Test();
                stopper = new Stopper()
                    .BindSignal()
                    .BindCancelKey()
                    .Wait();
                connector.Dispose();
            } finally
            {
                stopper?.SignalWeakUp();
            }
        }

        public static async void Test()
        {
            await Task.Delay(2000);
            Log.I.Info("Send Message");
            var heroService = new HeroService();
            var hero = await heroService.GetHero(999);
            Console.WriteLine(hero);
        }
    }
}