using System;
using Evil.Provide;
using Microsoft.Extensions.Configuration;

namespace Game
{
    public class CmdLine
    {
        public static CmdLine I;


        [ConfigurationKeyName("pvid")] 
        public ushort Pvid { get; set; } = 1;
        [ConfigurationKeyName("provider")]
        public string Provider { get; set; } = ":10001";

        public Provider[] Providers { get; set; }

        public static void Init(string[] args)
        {
            if (args.Length == 0)
            {
                I = new CmdLine().Init();
                return;
            }
            var builder = new ConfigurationBuilder().AddCommandLine(args);
            var configuration = builder.Build();
            I = configuration.Get<CmdLine>() ?? throw new Exception("cmdLineArgs parse failed");
            I.Init();
        }

        private CmdLine Init()
        {
            Providers = Provide.ParseProvider(Provider);
            return this;
        }
    }
}