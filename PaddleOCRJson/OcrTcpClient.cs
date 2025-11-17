#region

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#endregion

namespace PaddleOCRJson;

public class OcrTcpClient : OcrClient
{
    private readonly IPEndPoint _engineEp;
    private readonly object _connectionLock = new();
    private TcpClient? _tcpClient;
    private NetworkStream? _networkStream;
    private StreamReader? _streamReader;
    private StreamWriter? _streamWriter;

    public OcrTcpClient(IPEndPoint ep)
    {
        _engineEp = ep;
    }

    private void EnsureConnected()
    {
        if (IsConnected())
            return;

        lock (_connectionLock)
        {
            if (IsConnected())
                return;

            CloseConnection();

            _tcpClient = new TcpClient();
            _tcpClient.Connect(_engineEp);
            _networkStream = _tcpClient.GetStream();
            _networkStream.ReadTimeout = 5000;
            _streamReader = new StreamReader(_networkStream);
            _streamWriter = new StreamWriter(_networkStream) { AutoFlush = true };
        }
    }

    private bool IsConnected()
    {
        try
        {
            if (_tcpClient == null || !_tcpClient.Connected)
                return false;

            var client = _tcpClient.Client;
            if (client == null)
                return false;

            if (client.Poll(0, SelectMode.SelectRead))
            {
                var buffer = new byte[1];
                if (client.Receive(buffer, SocketFlags.Peek) == 0)
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private void CloseConnection()
    {
        try
        {
            _streamWriter?.Dispose();
            _streamReader?.Dispose();
            _networkStream?.Dispose();
            _tcpClient?.Close();
        }
        catch
        {
            // Ignore errors during cleanup
        }
        finally
        {
            _streamWriter = null;
            _streamReader = null;
            _networkStream = null;
            _tcpClient = null;
        }
    }

    protected override string SendCommand(string command, int msTimeout = 5000)
    {
        string resp = string.Empty;
        ExecuteEvent.WaitOne();
        try
        {
            var retryCount = 0;
            const int maxRetries = 2;

            while (retryCount <= maxRetries)
            {
                try
                {
                    EnsureConnected();

                    if (_networkStream != null)
                        _networkStream.ReadTimeout = msTimeout;

                    if (_streamWriter == null || _streamReader == null)
                        throw new InvalidOperationException("Connection not established");

                    _streamWriter.WriteLine(command);
                    resp = _streamReader.ReadLine() ?? string.Empty;
                    break;
                }
                catch (IOException)
                {
                    CloseConnection();
                    retryCount++;
                    if (retryCount > maxRetries)
                        throw;
                    Thread.Sleep(100);
                }
                catch (SocketException)
                {
                    CloseConnection();
                    retryCount++;
                    if (retryCount > maxRetries)
                        throw;
                    Thread.Sleep(100);
                }
            }
        }
        finally
        {
            ExecuteEvent.Set();
        }

        return resp;
    }

    protected override void Dispose(bool disposing)
    {
        if (Disposed)
            return;

        if (disposing)
        {
            lock (_connectionLock)
            {
                CloseConnection();
            }
        }

        base.Dispose(disposing);
    }
}