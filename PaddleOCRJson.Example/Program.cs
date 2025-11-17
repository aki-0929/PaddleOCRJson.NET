using System;
using System.IO;
using System.Net;
using PaddleOCRJson;

namespace PaddleOCRJson.Example;

internal class Program
{
    private const string EnginePath = @"D:\PaddleOCR-json\PaddleOCR-json.exe";

    private static void Main(string[] args)
    {
        Console.WriteLine("PaddleOCRJson.NET 示例程序");
        Console.WriteLine("========================");
        Console.WriteLine();

        if (!File.Exists(EnginePath))
        {
            Console.WriteLine($"错误: 引擎文件不存在: {EnginePath}");
            Console.WriteLine("请确保 PaddleOCR-json.exe 存在于指定路径");
            Console.ReadKey();
            return;
        }

        try
        {
            Console.WriteLine("请选择测试模式:");
            Console.WriteLine("1. 管道模式 (Pipe Mode)");
            Console.WriteLine("2. TCP模式 (TCP Mode)");
            Console.Write("请输入选项 (1 或 2): ");
            var choice = Console.ReadLine();

            Console.WriteLine();
            if (choice == "1")
            {
                TestPipeMode();
            }
            else if (choice == "2")
            {
                TestTcpMode();
            }
            else
            {
                Console.WriteLine("无效选项，默认使用管道模式");
                TestPipeMode();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine();
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
    }

    private static void TestPipeMode()
    {
        Console.WriteLine("=== 测试管道模式 ===");
        var startupArgs = OcrEngineStartupArgs.WithPipeMode(EnginePath);
        using var engine = new OcrEngine(startupArgs);
        using var client = engine.CreateClient();

        Console.WriteLine("引擎已启动 (管道模式)");
        TestOcr(client);
    }

    private static void TestTcpMode()
    {
        Console.WriteLine("=== 测试TCP模式 ===");
        var startupArgs = OcrEngineStartupArgs
            .WithTcpMode(EnginePath, IPAddress.Loopback, 0)
            .WithCpuThreads(4);
        using var engine = new OcrEngine(startupArgs);
        using var client = engine.CreateClient();

        Console.WriteLine("引擎已启动 (TCP模式)");
        TestOcr(client);
    }

    private static void TestOcr(OcrClient client)
    {
        var testCount = 0;
        while (true)
        {
            testCount++;
            Console.WriteLine();
            Console.WriteLine($"========== 第 {testCount} 次测试 ==========");
            Console.WriteLine("请输入要识别的图片路径 (输入 'q' 或 'quit' 退出):");
            var imagePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(imagePath) || 
                imagePath.Equals("q", StringComparison.OrdinalIgnoreCase) || 
                imagePath.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("退出测试");
                break;
            }

            if (!File.Exists(imagePath))
            {
                Console.WriteLine("图片路径无效或文件不存在");
                continue;
            }

            Console.WriteLine($"正在识别图片: {imagePath}");
            var startTime = DateTime.Now;
            try
            {
                var result = client.FromImageFile(imagePath);
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                Console.WriteLine();
                Console.WriteLine($"识别完成 (耗时: {elapsed:F2}ms)");
                Console.WriteLine("识别结果:");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                Console.WriteLine($"识别失败 (耗时: {elapsed:F2}ms): {ex.Message}");
            }
        }
    }
}

