namespace Generator.Util;

public class ProtoUtil
{
    public static string CalProtoFileNameByNamespace(string name)
    {
        // 将.换成_，且转换为小写
        return name.Replace(".", "_").ToLower() + ".proto";
    }
}