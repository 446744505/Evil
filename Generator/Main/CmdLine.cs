using Microsoft.Extensions.Configuration;

namespace Generator
{
    internal class CmdLine
    {
        public static CmdLine I = null!;

        [ConfigurationKeyName("interface")]
        public string InterfaceProject { get; set; } = null!;
        [ConfigurationKeyName("codeOut")] 
        public string CodeOutputPath { get; set; } = null!;
        [ConfigurationKeyName("node")]
        public string Node { get; set; } = null!;

        public static void Init(string[] args)
        {
            var builder = new ConfigurationBuilder().AddCommandLine(args);
            var configuration = builder.Build();
            I = configuration.Get<CmdLine>() ?? throw new System.Exception("cmdLineArgs parse failed");
        }
    }
}