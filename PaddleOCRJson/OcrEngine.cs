using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace PaddleOCRJson
{
    public class OcrEngine : IDisposable
    {
        private readonly OcrEngineStartupArgs _engineStartupArgs;
        private readonly string _enginePath;
        private readonly string _engineWorkPath;
        private readonly OcrEngineMode _engineMode;
        private readonly AutoResetEvent _engineStartupEvent;
        private readonly StringBuilder _outputSb;
        private bool _isEngineStarted;
        private Process _engineProcess;
        private ushort _enginePort;
        private bool _disposed;

        public ushort Port => _enginePort;
        public Action<string> OnEngineOutputReceived { get; set; }


        public OcrEngine(OcrEngineStartupArgs engineStartupArgs)
        {
            _engineStartupArgs = engineStartupArgs;
            _enginePath = _engineStartupArgs.EnginePath;
            _engineWorkPath = Path.GetDirectoryName(_enginePath);
            _engineMode = _engineStartupArgs.EngineMode;
            _engineStartupEvent = new AutoResetEvent(false);
            _outputSb = new StringBuilder();
            _isEngineStarted = false;
            _enginePort = 0;
            _disposed = false;

            StartEngine();
        }


        private void StartEngine()
        {
            _engineProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _enginePath,
                    WorkingDirectory = _engineWorkPath,
                    Arguments = _engineStartupArgs.ToString(),
                    CreateNoWindow = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                },
            };
            _engineProcess.OutputDataReceived += OnEngineDataReceived;
            _engineProcess.ErrorDataReceived += OnEngineDataReceived;

            if (_engineProcess.Start())
            {
                _engineProcess.StandardInput.AutoFlush = true;
                _engineProcess.BeginOutputReadLine();
                _engineProcess.BeginErrorReadLine();
                _engineStartupEvent.WaitOne(5000);
            }

            if (!_isEngineStarted)
                throw new Exception($"Engine start failed:\n{_outputSb}");

            if (_engineMode == OcrEngineMode.Pipe)
                return;

            _engineProcess.CancelOutputRead();
            _engineProcess.CancelErrorRead();
        }

        private void OnEngineDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
                return;
            if (!_isEngineStarted)
                _outputSb.AppendLine(e.Data);
            switch (_engineMode)
            {
                case OcrEngineMode.Pipe:
                {
                    if (e.Data == OcrEngineValues.EngineInitCompleted)
                    {
                        _isEngineStarted = true;
                        _engineStartupEvent.Set();
                        break;
                    }

                    OnEngineOutputReceived?.Invoke(e.Data);
                    break;
                }
                case OcrEngineMode.Tcp:
                {
                    if (e.Data.StartsWith(OcrEngineValues.SocketInitCompleted))
                    {
                        var socketInfo = e.Data.Replace(OcrEngineValues.SocketInitCompleted, "").Split(':');
                        _enginePort = ushort.Parse(socketInfo[1]);
                        _isEngineStarted = true;
                        _engineStartupEvent.Set();
                        break;
                    }

                    OnEngineOutputReceived?.Invoke(e.Data);
                    break;
                }
            }
        }

        internal void WriteLine(string data)
        {
            _engineProcess.StandardInput.WriteLine(data);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _engineProcess?.Kill();
            _engineProcess?.Dispose();
            _disposed = true;
        }

        public OcrClient CreateClient()
        {
            return new OcrClient(this);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}