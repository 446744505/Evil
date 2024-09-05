using System;
using Microsoft.Extensions.Configuration;

namespace Map
{
    public class CmdLine
    {
        public static CmdLine I;


        [ConfigurationKeyName("pvid")] 
        public ushort Pvid { get; set; } = 2;
        [ConfigurationKeyName("etcd")]
        public string Etcd { get; set; } = "https://127.0.0.1:2379";

        public static void Init(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            var builder = new ConfigurationBuilder().AddCommandLine(args);
            var configuration = builder.Build();
            I = configuration.Get<CmdLine>() ?? throw new Exception("cmdLineArgs parse failed");
        }
    }
}