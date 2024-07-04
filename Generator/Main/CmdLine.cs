using Microsoft.Extensions.Configuration;

namespace Generator;

public class CmdLine
{
    public static CmdLine I;
    
    public static void Init(string[] args)
    {
        var builder = new ConfigurationBuilder().AddCommandLine(args);
        var configuration = builder.Build();
        I = configuration.Get<CmdLine>() ?? throw new Exception("cmdLineArgs parse failed");
    }
}