# PaddleOCRJson.NET
[PaddleOCR-json v1.3.1](https://github.com/hiroi-sora/PaddleOCR-json) .net 的封装

## NuGet 安装
```bash
dotnet add package PaddleOCRJson.NET
```

或通过 NuGet 包管理器搜索安装: `PaddleOCRJson.NET`

## 源码方式
克隆源码, 然后添加项目引用 `PaddleOCRJson.csproj` .

## 特性
- 支持管道模式和 TCP 模式
- TCP 客户端支持长连接和自动重连
- 简洁的 API 设计，支持链式调用
- 无外部依赖
- 识别结果为 JSON 字符串，需要用户自己解析

## .NET Standard version
2.0, 2.1.


## 依赖
无

## 使用方法


### 1. 设置引擎路径(远程部署忽略, 直接看2.3即可)
```
var enginePath = "PaddleOCR-json/PaddleOCR-json.exe";
```

###	2. 设置引擎模式和其他参数
#### 2.1. 管道模式
```
var startupArgs = OcrEngineStartupArgs.WithPipeMode(enginePath);
using var engine = new OcrEngine(startupArgs);
using var cli = engine.CreateClient(); // 或者: using var cli = new OcrPipeClient(engine)
```

#### 2.2. 自托管TCP模式
```
var startupArgs = OcrEngineStartupArgs.WithTcpMode(enginePath, IPAddress.Loopback, 0); // 设置监听地址和端口号
using var engine = new OcrEngine(startupArgs);
using var cli = engine.CreateClient(); // 或者: using var cli = new OcrTcpClient(new IPEndPoint(IPAddress.Loopback, engine.Port))
```

#### 2.3. 远程TCP模式
```
using var cli = new OcrTcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345)); // IP和端口嘛
```

#### 2.4. 更多参数
```
startupArgs
	.WithEnableMkldnn(true)             // 启用Mkldnn
	.WithCpuThreads(10)                 // 设置线程数
	.WithCpuMem(2000)                   // CPU内存限制(MB)
	.WithPrecision("fp32")              // 精度: fp32/fp16/int8
	.WithDetModelDir("models/...")      // DET模型路径
	.WithRecModelDir("models/...")      // REC模型路径
	.WithCustom("arg_int",1)            // 自定义int参数
	.WithCustom("arg_bool",true)        // 自定义bool参数
	.WithCustom("arg_str","xxxx");      // 自定义str参数
```

**参数说明请参考**: [PaddleOCR-json 常用配置参数说明](https://github.com/hiroi-sora/PaddleOCR-json/tree/main?tab=readme-ov-file#%E5%B8%B8%E7%94%A8%E9%85%8D%E7%BD%AE%E5%8F%82%E6%95%B0%E8%AF%B4%E6%98%8E)

所有可用参数方法请参考 [OcrEngineStartupArgs.cs](https://github.com/aki-0929/PaddleOCRJson.NET/blob/main/PaddleOCRJson/OcrEngineStartupArgs.cs)

### 3. 执行操作 
```
var ret = string.Empty;
ret = cli.FromImageBytes(new byte[]{...});  // byte[] 会转到 base64
ret = cli.FromBase64("...");                // base64
ret = cli.FromImageFile("test.png");        // 直接使用文件路径
ret = cli.FromClipboard();                  // 没啥好说的
```

### 4. 解析结果

#### 4.1. 返回值参考 [原作者的文档](https://github.com/hiroi-sora/PaddleOCR-json/tree/main#%E8%BF%94%E5%9B%9E%E5%80%BC%E8%AF%B4%E6%98%8E)

#### 4.2. 解析示例

返回值是 JSON 字符串，格式如下：
```json
{
  "code": 100,
  "data": [
    {
      "box": [[x1, y1], [x2, y2], [x3, y3], [x4, y4]],
      "score": 0.95,
      "text": "识别的文本"
    }
  ]
}
```

可以使用任何 JSON 库来解析结果，例如使用 `System.Text.Json`：

```csharp
using System.Text.Json;

var result = client.FromImageFile("test.png");
var jsonDoc = JsonDocument.Parse(result);
var code = jsonDoc.RootElement.GetProperty("code").GetInt32();
if (code == 100)
{
    var data = jsonDoc.RootElement.GetProperty("data");
    foreach (var item in data.EnumerateArray())
    {
        var text = item.GetProperty("text").GetString();
        var score = item.GetProperty("score").GetDouble();
        Console.WriteLine($"{text} (置信度: {score})");
    }
}
```

**注意**: 本库不包含 JSON 解析功能，需要用户自己解析（不想引入任何依赖）。
