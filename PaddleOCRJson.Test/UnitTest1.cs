using System.IO.Compression;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using PaddleOCRJson.Enums.StartupArgs;

namespace PaddleOCRJson.Test
{
    [TestClass]
    public class UnitTest1
    {
        private readonly string _runtimePath;
        private readonly string _enginePath;
        private readonly string _imagePath;
        private readonly byte[] _imageBytes;
        private readonly string _imageBase64;
        private readonly string _result;

        public UnitTest1()
        {
            var asmLocPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var defaultRuntimeZipPath = Path.Combine(asmLocPath, "default_runtime.zip");
            _runtimePath = Path.Combine(asmLocPath, "runtime");
            _enginePath = Path.Combine(_runtimePath, "PaddleOCR-json.exe");
            _imagePath = Path.Combine(_runtimePath, "images", "image1.png");
            _result = "M8k2";

            using (var zip = new ZipArchive(File.OpenRead(defaultRuntimeZipPath)))
            {
                if (Directory.Exists(_runtimePath))
                    Directory.Delete(_runtimePath, true);
                Directory.CreateDirectory(_runtimePath);
                zip.ExtractToDirectory(_runtimePath);
            }

            _imageBytes = File.ReadAllBytes(_imagePath);
            _imageBase64 = Convert.ToBase64String(_imageBytes);
        }

        ~UnitTest1()
        {
            Directory.Delete(_runtimePath, true);
        }

        public void Test(OcrClient client)
        {
            {
                var result = JsonConvert.DeserializeObject<OcrResult>(client.FromImageBytes(_imageBytes));
                Assert.IsTrue(result.Succeed && result.OcrData[0].Text == _result);
            }
            {
                var result = JsonConvert.DeserializeObject<OcrResult>(client.FromBase64(_imageBase64));
                Assert.IsTrue(result.Succeed && result.OcrData[0].Text == _result);
            }
            {
                var result = JsonConvert.DeserializeObject<OcrResult>(client.FromImageFile(_imagePath));
                Assert.IsTrue(result.Succeed && result.OcrData[0].Text == _result);
            }
            //{
            //    var result = JsonConvert.DeserializeObject<OcrResult>(client.FromClipboard());
            //    Assert.IsTrue(result.Succeed && result.OcrData[0].Text == _result);
            //}
        }

        [TestMethod]
        public void OcrEngine_Pipe()
        {
            var startupArgs = OcrEngineStartupArgs.WithPipeMode(_enginePath);
            using var engine = new OcrEngine(startupArgs);
            using var cli = engine.CreateClient();
            Test(cli);
        }

        [TestMethod]
        public void OcrEngine_Tcp_InternalEngine()
        {
            var startupArgs = OcrEngineStartupArgs.WithTcpMode(_enginePath, ListenAddresses.Loopback, 0);
            using var engine = new OcrEngine(startupArgs);
            using var cli = engine.CreateClient();
            Test(cli);
        }

        [TestMethod]
        public void OcrEngine_Tcp_ExternalEngine()
        {
            using var cli = new OcrClient(new IPEndPoint(IPAddress.Loopback, 12345));
            Test(cli);
        }

        [TestMethod]
        public void OcrEngine_z7z8()
        {
            var startupArgs = OcrEngineStartupArgs
                .WithPipeMode(_enginePath)
                .CpuThreads(8)
                .ConfigPath("models/config_chinese.txt");
            Console.WriteLine(startupArgs.ToString());
            using var engine = new OcrEngine(startupArgs);
            using var cli = engine.CreateClient();
            Test(cli);
        }
    }
}