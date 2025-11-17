#region

using System.Threading;

#endregion

namespace PaddleOCRJson;

public class OcrPipeClient : OcrClient
{
    private readonly OcrEngine _engine;

    public OcrPipeClient(OcrEngine engine)
    {
        _engine = engine;
    }

    protected override string SendCommand(string command, int msTimeout = 5000)
    {
        var rep = string.Empty;
        ExecuteEvent.WaitOne();
        try
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
        finally
        {
            _engine.OnEngineOutputReceived = null;
            ExecuteEvent.Set();
        }

        return rep;
    }

    protected override void Dispose(bool disposing)
    {
        if (Disposed)
            return;
        _engine?.Dispose();
        base.Dispose(disposing);
    }
}