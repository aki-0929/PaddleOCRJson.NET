using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PaddleOCRJson
{
    public class OcrClient : IDisposable
    {
        private readonly OcrEngine _engine;
        private readonly IPEndPoint _engineEp;
        private readonly ManualResetEvent _executeEvent;

        private bool _disposed;

        public OcrClient()
        {
            _engine = null;
            _engineEp = null;
            _executeEvent = new ManualResetEvent(true);
            _disposed = false;
        }

        public OcrClient(OcrEngine engine) : this()
        {
            _engine = engine;
            if (_engine.Port != 0)
                _engineEp = new IPEndPoint(IPAddress.Loopback, _engine.Port);
        }

        public OcrClient(IPEndPoint ep) : this()
        {
            _engineEp = ep;
        }


        private string SendCommand(string command, int msTimeout = 5000)
        {
            var rep = string.Empty;
            _executeEvent.WaitOne();
            try
            {
                if (_engineEp == null)
                {
                    var resultEvent = new AutoResetEvent(false);
                    _engine.OnEngineOutputReceived = (output) =>
                    {
                        rep = output;
                        resultEvent.Set();
                    };
                    _engine.WriteLine(command);
                    resultEvent.WaitOne(msTimeout);
                }
                else
                {
                    using var tcpClient = new TcpClient();
                    tcpClient.Connect(_engineEp);
                    var netStream = tcpClient.GetStream();
                    netStream.ReadTimeout = msTimeout;
                    using (var sw = new StreamWriter(netStream))
                    using (var sr = new StreamReader(netStream))
                    {
                        sw.WriteLine(command);
                        sw.Flush();
                        rep = sr.ReadToEnd();
                    }

                    tcpClient.Close();
                }
            }

            finally
            {
                if (_engine != null)
                    _engine.OnEngineOutputReceived = null;
                _executeEvent.Set();
            }

            return rep;
        }

        public string FromBase64(string base64)
        {
            return SendCommand($"{{\"image_base64\":\"{base64}\"}}");
        }

        public string FromImageBytes(byte[] imageBytes)
        {
            // 偷懒
            return FromBase64(Convert.ToBase64String(imageBytes));
        }

        public string FromImageFile(string imageFilePath)
        {
            // 偷大懒
            return FromImageBytes(File.ReadAllBytes(imageFilePath));
            // 可以不处理转义
            // return SendCommand($"{{\"image_path\":\"{imageFilePath}\"}}");
        }

        public string FromClipboard()
        {
            return SendCommand($"{{\"image_path\":\"clipboard\"}}");
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _engine?.Dispose();
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}