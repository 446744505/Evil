namespace Generator
{
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
        /// 生成的.proto文件和.cs文件的目录
        /// </summary>
        public const string ProtoPath = "Proto";
        /// <summary>
        /// 生成的message文件的目录
        /// </summary>
        public const string MessagePath = "Message";
        /// <summary>
        /// Interface项目里存在所有Attribute的目录
        /// </summary>
        public const string AttributesDir = "Attributes";
        /// <summary>
        /// 消息注册类的文件名
        /// </summary>
        public const string MessageRegister = "MessageRegister";
    }
}