using System.IO;
using Generator.Context;
using Generator.Kind;
using Generator.Proto;
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
            var registerBody = new Writer();
            // 遍历所有message，每个message生成一个cs文件
            foreach (var messageName in m_Gc.ProtocolMessageNames)
            {
                var kind = m_Gc.FindIdentiferKind(messageName);
                var registerLine = CreateMessageFile(kind, messagePath);
                registerBody.WriteLine(registerLine);
            }

            // 生成MessageRegister.cs
            var registerPath = Path.Combine(messagePath, $"{Files.MessageRegister}{Files.CodeFileSuffix}");
            var registerCode = $@"
namespace NetWork
{{
    public class MessageRegister : IMessageRegister
    {{
        public void Register(IMessageProcessor processor)
        {{
            {registerBody}
        }}
    }}
}}
";
            File.WriteAllText(registerPath, registerCode);
        }

        private string CreateMessageFile(BaseIdentiferKind kind, string messagePath)
        {
            var filePath = Path.Combine(messagePath, $"{kind.Name}{Files.CodeFileSuffix}");
            var namespaceName = $"{kind.NamespaceName()}";
            var messageId = MessageIdGenerator.CalMessageId(kind.Name);
            var parent = "NetWork.Message";
            // 是rpc req
            if (kind is ReqClassKind reqClassKind && !string.IsNullOrEmpty(reqClassKind.AckFullName))
            {
                parent = $"NetWork.Rpc<{reqClassKind.AckFullName}>";
            }
            var code = $@"
namespace {namespaceName}
{{
    public partial class {kind.Name} : {parent}
    {{
        public override uint MessageId => {messageId};
    }}
}}
              ";
            File.WriteAllText(filePath, code);
            m_Gc.Log($"生成文件: {filePath}");
            return $"processor.Register({messageId}, () => new {namespaceName}.{kind.Name}());";
        }
    }
}