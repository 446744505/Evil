namespace Generator;

public class Files
{
    /// <summary>
    /// 通用换行符
    /// </summary>
    public static string NewLine = "\n";
    
    public const string CodeFileSuffix = ".cs";
    /// <summary>
    /// 生成的文件名后缀
    /// </summary>
    public const string GeneratorFileSuffix = $".g{CodeFileSuffix}";
    /// <summary>
    /// 生成的.proto文件的临时路径
    /// </summary>
    public const string ProtoFileTmpPath = "ProtoTmp";
}