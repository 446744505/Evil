
using Generator.Context;
using Microsoft.CodeAnalysis.MSBuild;

namespace Generator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // 解析命令行参数
            CmdLine.Init(args);
            // 打开项目
            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync(CmdLine.I.InterfaceProject).Result;
            // 初始化全局上下文
            var gc = new GloableContext(CmdLine.I.CodeOutputPath);
            // 删除旧的生成文件
            gc.CleanGeneratedFiles();
            try
            {
                var progress = new Progress(gc);
                // 遍历所有源文件并处理
                foreach (var document in project.Documents)
                {
                    try
                    {
                        var fc = progress.ParseDocument(document);
                        progress.CreateFile(fc);
                    }
                    catch (System.Exception e)
                    {
                        throw new System.Exception($"解析文件{document.FilePath}失败", e);
                    }
                }
                // 编译
                progress.Compile();

                // 生成.proto文件
                var pg = new ProtoGenerator(gc);
                pg.GenerateMeta(gc);
            } catch (System.Exception e)
            {
                gc.Exception(e);
            }
        }
    }
}


