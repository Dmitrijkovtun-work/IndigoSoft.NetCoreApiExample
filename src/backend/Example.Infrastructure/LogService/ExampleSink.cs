using Serilog.Core;
using Serilog.Events;

namespace Example.Infrastructure.LogService;

public sealed class ExampleSink: ILogEventSink, IDisposable
{
    // for init logs collector systems aka kafka, redis, etc.
    static ExampleSink()
    {
        Console.WriteLine("Logger static constructor ");
    }

    public void Emit(LogEvent logEvent)
    {
        Console.WriteLine($"{logEvent.SpanId} - {logEvent.MessageTemplate}");
    }

    public void Dispose() { }
}
