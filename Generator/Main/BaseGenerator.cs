using System.Collections.Generic;
using System.IO;
using Generator.Context;
using Generator.Kind;

namespace Generator
{
    public abstract class BaseGenerator<T> where T : NamespaceKind
    {
        private string m_Path { get; }
        protected GloableContext Gc { get; }
        protected GeneratorContext Context { get; }
        protected string OutPath => Path.Combine(Gc.OutPath, m_Path);
        
        public BaseGenerator(GloableContext gc, GeneratorContext context, string path)
        {
            Gc = gc;
            m_Path = path;
            Context = context;
            context.IdentiferFind = name =>
            {
                return gc.FindIdentiferKind<T>(name);
            };
        }

        protected Dictionary<string, List<BaseIdentiferKind>> NamespaceClassKinds()
        {
            // 从context中获取所有的namespace和class
            Dictionary<string, List<BaseIdentiferKind>> namespaceClassKinds = new();
            foreach (var fc in Gc.FileContexts)
            {
                foreach (var namespaceKind in fc.FindNamespaceKinds<T>())
                {
                    if (namespaceClassKinds.TryGetValue(namespaceKind.Name, out var classKinds))
                    {
                        classKinds.AddRange(namespaceKind.Children());
                    }
                    else
                    {
                        namespaceClassKinds.Add(namespaceKind.Name, namespaceKind.Children());
                    }
                }
            }

            return namespaceClassKinds;
        }

        public void Generate()
        {
            // 清空并创建文件夹
            var outPath = OutPath;
            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
            }
            Directory.CreateDirectory(outPath);
            Generate0();
        }
        
        protected abstract void Generate0();
    }
}