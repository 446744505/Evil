using System.IO;
using Generator.Context;
using Generator.Kind;
using Generator.Util;

namespace Generator.Message
{
    public class MessageGenerator
    {
        private readonly GloableContext m_Gc;

        public MessageGenerator(GloableContext gc)
        {
            m_Gc = gc;
        }

        public void GenerateMessage()
        {
            // 清空并创建文件夹
            var messagePath = Path.Combine(m_Gc.OutPath, Files.MessagePath);
            if (Directory.Exists(messagePath))
            {
                Directory.Delete(messagePath, true);
            }
            Directory.CreateDirectory(messagePath);
            // 遍历所有message，每个message生成一个cs文件
            foreach (var messageName in m_Gc.ProtocolMessageNames)
            {
                var kind = m_Gc.FindIdentiferKind(messageName);
                CreateMessageFile(kind, messagePath);
            }
        }

        private void CreateMessageFile(BaseIdentiferKind kind, string messagePath)
        {
            var filePath = Path.Combine(messagePath, $"{kind.Name}{Files.CodeFileSuffix}");
            var namespaceName = $"{kind.NamespaceName()}.{Namespaces.ProtoNamespace}";
            var code = $@"
namespace {namespaceName}
{{
    public partial class {kind.Name} : NetWork.Message
    {{
        public override uint MessageId => {MessageIdGenerator.CalMessageId(kind.Name)};
    }}
}}
              ";
            File.WriteAllText(filePath, code);
            m_Gc.Log($"生成文件: {filePath}");
        }
    }
}