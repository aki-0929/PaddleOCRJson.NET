# PaddleOCRJson.NET
[PaddleOCR-json v1.3.1](https://github.com/hiroi-sora/PaddleOCR-json) .net 的封装

# Nuget方式
搜索安装: PaddleOCRJson.NET .

# 源码方式
克隆源码, 然后添加项目引用 PaddleOCRJson.csproj .

# 碎碎念

坏了!!! Nuget项目地址写错了, 还不让改.

代码没注释 真烂!

先这样吧 反正没啥很复杂的操作. 其他看看[原作者的文档](https://github.com/hiroi-sora/PaddleOCR-json/tree/main/docs)好啦

识别结果为json字符串, 需要用户自己解析(不想引入任何依赖).

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
var startupArgs = OrEngineStartupArgs.WithPipeMode(enginePath);
using var engine = new OcrEngine(startupArgs);
using var cli = engine.CreateClient(); // using var cli = new OcrClient(engine)
```

#### 2.2. 自托管TCP模式
```
var startupArgs = OcrEngineStartupArgs.WithTcpMode(_enginePath, ListenAddresses.Loopback, 0); // 设置监听和端口号
using var engine = new OcrEngine(startupArgs);
using var cli = engine.CreateClient(); // using var cli = new OcrClient(engine)
```

#### 2.3. 远程TCP模式
```
using var cli = new OcrClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345)); // IP和端口嘛
```

#### 2.4. 更多参数(主要是自用, 随缘更新, 没更新的话下面的自定义参数就可以凑活凑活)
```
startupArgs
	.EnableMkldnn(true)             // 启用Mkldnn
	.CpuThreads(10)                 // 设置线程数
	.Custom("arg_int",1)            // 自定义int参数
	.Custom("arg_bool",true)        // 自定义bool参数
	.Custom("arg_str","xxxx");      // 自定义str参数
```

### 3. 执行操作 
```
var ret = string.Empty;
ret = cli.FromImageBytes(new byte[]{...});  // byte[] 会转到 base64
ret = cli.FromBase64("...");                // base64
ret = cli.FromImageFile("test.png");        // 为了避免转义(偷懒)的问题, 读入byte[]后也会转到base64.
ret = cli.FromClipboard();                  // 没啥好说的
```

### 4. 解析结果

#### 4.1. 返回值参考 [原作者的文档](https://github.com/hiroi-sora/PaddleOCR-json/tree/main#%E8%BF%94%E5%9B%9E%E5%80%BC%E8%AF%B4%E6%98%8E)

#### 4.2. 也可以参考 [UnitTest1.cs](https://github.com/aki-0929/PaddleOCRJson.NET/blob/main/PaddleOCRJson.Test/UnitTest1.cs#L48)

Q: 为什么不写解析?
 
A: 想要干干净净.

#### [OcrResult.cs](https://github.com/aki-0929/PaddleOCRJson.NET/blob/master/PaddleOCRJson.Test/OcrResult.cs)
```
using Newtonsoft.Json;

namespace PaddleOCRJson.Test
{
    public class OcrResult
    {
        public int Code { get; set; }
        public object Data { get; set; }

        [JsonIgnore] public bool Succeed => Code == 100;

        [JsonIgnore]
        public OcrData[] OcrData =>
            Succeed ? JsonConvert.DeserializeObject<OcrData[]>(Data.ToString()) : null;
    }
}
```


#### [OcrData.cs](https://github.com/aki-0929/PaddleOCRJson.NET/blob/master/PaddleOCRJson.Test/OcrData.cs)
```
namespace PaddleOCRJson.Test
{
    public class OcrData
    {
        public int[][] Box { get; set; }
        public double Score { get; set; }
        public string Text { get; set; }
    }
}
```
