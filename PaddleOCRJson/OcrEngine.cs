#region

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

#endregion

namespace PaddleOCRJson;

public class OcrEngine : IDisposable
{
    private readonly OcrEngineMode _engineMode;
    private readonly Process _engineProcess;
    private readonly AutoResetEvent _engineStartupEvent;
    private readonly StringBuilder _outputSb;
    private bool _disposed;
    private ushort _enginePort;
    private bool _isEngineStarted;


    public OcrEngine(OcrEngineStartupArgs engineStartupArgs)
    {
        var enginePath = engineStartupArgs.EnginePath;
        var engineWorkPath = Path.GetDirectoryName(enginePath) ?? throw new Exception("Invalid engine work directory.");

        _engineMode = engineStartupArgs.EngineMode;
        _engineStartupEvent = new AutoResetEvent(false);
        _outputSb = new StringBuilder();
        _isEngineStarted = false;
        _enginePort = 0;
        _disposed = false;

        _engineProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = enginePath,
                WorkingDirectory = engineWorkPath,
                Arguments = engineStartupArgs.ToString(),
                CreateNoWindow = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
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

    public Action<string>? OnEngineOutputReceived { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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
        _engineProcess.Kill();
        _engineProcess.Dispose();
        _disposed = true;
    }

    public OcrClient CreateClient()
    {
        return _engineMode == OcrEngineMode.Pipe
            ? new OcrPipeClient(this)
            : new OcrTcpClient(new IPEndPoint(IPAddress.Loopback, _enginePort));
    }
}