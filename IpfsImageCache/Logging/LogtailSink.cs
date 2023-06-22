// Derived from an MIT Licensed implementation by Egor Pavlikhin https://github.com/egorpavlikhin/Serilog.Sinks.Logtail

using Logtail;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace IpfsImageCache.Logging;

public class LogtailSink : ILogEventSink, IDisposable
{
    readonly IFormatProvider? _formatProvider;
    readonly Drain _logtail;

    public LogtailSink(
        IFormatProvider? formatProvider,
        string sourceToken,
        string endpoint = "https://in.logtail.com",
        int retries = 10,
        int flushPeriodMilliseconds = 250,
        int maxBatchSize = 100)
    {
        _formatProvider = formatProvider;

        var client = new Client(sourceToken, endpoint, retries: retries);

        _logtail = new Drain(
            client,
            period: TimeSpan.FromMilliseconds(flushPeriodMilliseconds),
            maxBatchSize
        );
    }

    public void Dispose()
    {
        _logtail?.Stop().Wait();
        GC.SuppressFinalize(this);
    }

    public void Emit(LogEvent logEvent)
    {
        var contextDictionary = logEvent.Properties.ToDictionary(x => x.Key, x => (object)x);

        var log = new Logtail.Log()
        {
            Timestamp = logEvent.Timestamp,
            Message = logEvent.RenderMessage(_formatProvider),
            Level = logEvent.Level.ToString(),
            Context = contextDictionary,
        };

        _logtail.Enqueue(log);
    }
}

public static class LogtailSeqSinkExtensions
{
    public static LoggerConfiguration LogtailSink(
        this LoggerSinkConfiguration loggerConfiguration,
        string sourceToken,
        IFormatProvider? formatProvider = null,
        string endpoint = "https://in.logtail.com",
        int retries = 10,
        int flushPeriodMilliseconds = 250,
        int maxBatchSize = 1000)
    {
        return loggerConfiguration.Sink(new LogtailSink(formatProvider, sourceToken, endpoint, retries, flushPeriodMilliseconds, maxBatchSize));
    }
}
