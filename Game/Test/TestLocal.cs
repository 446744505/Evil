using System;
using System.Threading;
using System.Threading.Tasks;

namespace Game;

public class TestLocal
{
    // 定义一个 AsyncLocal 变量
    private static AsyncLocal<string> asyncLocalValue = new AsyncLocal<string>();

    static async Task Main(string[] args)
    {
        // 设置 AsyncLocal 的值
        asyncLocalValue.Value = "初始值";
        Console.WriteLine($"{Thread.CurrentThread.Name} 主线程中的 AsyncLocal 值: {asyncLocalValue.Value}");
        
        // 启动一个异步任务
        await Task.Run(() =>
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} 异步任务1中的 AsyncLocal 值: {asyncLocalValue.Value}");
            // 清空 AsyncLocal 的值
            asyncLocalValue.Value = "修改值1"; // 或者 asyncLocalValue.Value = null;
            Console.WriteLine($"{Thread.CurrentThread.Name} 异步任务1修改后的 AsyncLocal 值: {asyncLocalValue.Value}");
        });
        
        // 启动一个异步任务
        await Task.Run(() =>
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} 异步任务2中的 AsyncLocal 值: {asyncLocalValue.Value}");
            // 清空 AsyncLocal 的值
            asyncLocalValue.Value = "修改值2"; // 或者 asyncLocalValue.Value = null;
            Console.WriteLine($"{Thread.CurrentThread.Name} 异步任务2修改后的 AsyncLocal 值: {asyncLocalValue.Value}");
        });
        
        // 在主线程中再次检查值
        Console.WriteLine($"{Thread.CurrentThread.Name} 主线程中的 AsyncLocal 值 (任务后): {asyncLocalValue.Value}");
    }

}