using System.Collections.Generic;
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
            // 所有ack
            var ackNames = new HashSet<string>();
            foreach (var pair in Gc.ProtocolMessageNames)
            {
                // 记录RpcAck消息
                if (pair.Value)
                {
                    ackNames.Add(pair.Key);
                }
            }

            var registerBody = new Writer(true, 3);
            // 遍历所有message，每个message生成一个cs文件
            foreach (var messageName in Gc.ProtocolMessageNames.Keys)
            {
                var kind = Gc.FindIdentiferKind<ProtoNamespaceKind>(messageName);
                var registerLine = CreateMessageFile((ProtoClassKind)kind, ackNames);
                registerBody.WriteLine(registerLine);
            }

            // 生成MessageRegister.cs
            var registerPath = Path.Combine(OutPath, $"{Files.MessageRegister}{Files.CodeFileSuffix}");
            var registerCode = $@"
{AutoGenerated}
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

        private string CreateMessageFile(ProtoClassKind kind, HashSet<string> ackNames)
        {
            var filePath = Path.Combine(OutPath, $"{kind.Name}{Files.CodeFileSuffix}");
            var namespaceName = $"{kind.NamespaceName()}";
            var messageId = MessageIdGenerator.CalMessageId(kind.Name);
            var parent = "NetWork.Message";
            if (ackNames.Contains(kind.Name))
            {
                parent = "NetWork.RpcAck";
            }
            // 是rpc req
            if (kind is ReqClassKind reqClassKind && !string.IsNullOrEmpty(reqClassKind.AckFullName))
            {
                parent = $"NetWork.Rpc<{reqClassKind.AckFullName}>";
            }
            // max size
            var maxSizeLine = string.Empty;
            if (kind.MaxSize > -1)
            {
                maxSizeLine = $"public override int MaxSize => {kind.MaxSize};";
            }
            var code = $@"
{AutoGenerated}
namespace {namespaceName}
{{
    public partial class {kind.Name} : {parent}
    {{
        [System.Text.Json.Serialization.JsonIgnore]
        public override uint MessageId => {messageId};
        {maxSizeLine}
{kind.GenConstFields(2)}
{kind.GenToString(2)}
    }}
}}
              ";
            File.WriteAllText(filePath, code);
            Gc.Log($"生成文件: {filePath}");
            return $"processor.Register({messageId}, () => new {namespaceName}.{kind.Name}());";
        }
    }
}