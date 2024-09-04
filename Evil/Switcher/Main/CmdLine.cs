using Microsoft.Extensions.Configuration;

namespace Evil.Switcher
{
    public class CmdLine
    {
        public static CmdLine I = null!;

        [ConfigurationKeyName("etcd")]
        public string Etcd { get; set; } = "https://127.0.0.1:2379";
        /// <summary>
        /// linker 端口
        /// </summary>
        [ConfigurationKeyName("linkerPort")]
        public int LinkerPort { get; set; } = 10000;
        
        /// <summary>
        /// provider 端口
        /// </summary>
        [ConfigurationKeyName("providerPort")]
        public int ProviderPort { get; set; } = 10001;

        /// <summary>
        /// 客户端心跳超时时间
        /// </summary>
        [ConfigurationKeyName("linkerTimeout")]
        public int LinkerSessionTimeout { get; set; } = 120; // 单位秒
        /// <summary>
        /// 服务器心跳超时时间
        /// </summary>
        [ConfigurationKeyName("providerTimeout")]
        public int ProviderSessionTimeout { get; set; } = 30; // 单位秒
        /// <summary>
        /// 一个linker 最多连接数
        /// </summary>
        [ConfigurationKeyName("maxSession")]
        public int MaxSessionCount { get; set; } = 4000;
        
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