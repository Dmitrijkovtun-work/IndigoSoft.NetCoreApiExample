using Serilog;
using Serilog.Configuration;

namespace Example.Infrastructure.LogService;

public static class ExampleSinkExtension
{
    public static LoggerConfiguration ExampleSink(this LoggerSinkConfiguration config)
    {
        return config.Sink(new ExampleSink());
    }
}
