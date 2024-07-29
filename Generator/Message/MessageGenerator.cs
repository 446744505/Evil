using System.IO;
using Evil.Util;
using Generator.Context;
using Generator.Kind;
using Generator.Proto;
using Generator.Util;

namespace Generator.Message
{
    public class MessageGenerator : BaseGenerator<ProtoNamespaceKind>
    {
        public MessageGenerator(GloableContext gc) : 
            base(gc, new GeneratorContext(), Files.MessagePath)
        {
        }
        protected override void Generate0()
        {
            var registerBody = new Writer();
            // 遍历所有message，每个message生成一个cs文件
            foreach (var messageName in Gc.ProtocolMessageNames)
            {
                var kind = Gc.FindIdentiferKind<ProtoNamespaceKind>(messageName);
                var registerLine = CreateMessageFile(kind);
                registerBody.WriteLine(registerLine);
            }

            // 生成MessageRegister.cs
            var registerPath = Path.Combine(OutPath, $"{Files.MessageRegister}{Files.CodeFileSuffix}");
            var registerCode = $@"
using NetWork;

namespace {Namespaces.ProtoNamespace}
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
            Gc.Log($"生成文件: {registerPath}");
            File.WriteAllText(registerPath, registerCode);
        }

        private string CreateMessageFile(BaseIdentiferKind kind)
        {
            var filePath = Path.Combine(OutPath, $"{kind.Name}{Files.CodeFileSuffix}");
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
            Gc.Log($"生成文件: {filePath}");
            return $"processor.Register({messageId}, () => new {namespaceName}.{kind.Name}());";
        }
    }
}