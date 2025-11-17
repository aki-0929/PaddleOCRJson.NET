#region

using System;
using System.IO;
using System.Threading;

#endregion

namespace PaddleOCRJson;

public abstract class OcrClient : IDisposable
{
    protected readonly ManualResetEvent ExecuteEvent = new(true);
    protected bool Disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected abstract string SendCommand(string command, int msTimeout = 5000);

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
        var escapedPath = imageFilePath.Replace("\\", "\\\\").Replace("\"", "\\\"");
        return SendCommand($"{{\"image_path\":\"{escapedPath}\"}}");
    }

    public string FromClipboard()
    {
        return SendCommand($"{{\"image_path\":\"clipboard\"}}");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Disposed)
            return;
        Disposed = true;
    }
}