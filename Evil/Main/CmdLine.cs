using Microsoft.Extensions.Configuration;

namespace Evil
{
    public class CmdLine
    {
        public static CmdLine I;
        
        /// <summary>
        /// 要启动的服务节点
        /// </summary>
        [ConfigurationKeyName("node")]
        public string Node { get; set; } = "switcher";
        
        [ConfigurationKeyName("etcd")]
        public string Etcd { get; set; } = "https://127.0.0.1:2379";
        
        public static void Init(string[] args)
        {
            if (args.Length == 0)
            {
                I = new CmdLine();
                return;
            }
            var builder = new ConfigurationBuilder().AddCommandLine(args);
            var configuration = builder.Build();
            I = configuration.Get<CmdLine>() ?? throw new Exception("cmdLineArgs parse failed");
        }
    }
}