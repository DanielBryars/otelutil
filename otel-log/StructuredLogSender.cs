using System;
using System.Net.Http;
using Grpc.Net.Client;
using OpenTelemetry.Proto.Collector.Logs.V1;
using OpenTelemetry.Proto.Logs.V1;
using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Resource.V1;

public class StructuredLogSender
{
    private readonly ILogger<StructuredLogSender> _logger;

    public StructuredLogSender(ILogger<StructuredLogSender> logger)
    {
        _logger = logger;
    }

    public async Task RunAsync()
    {
        var otelEndpoint = "http://localhost:4317";
        _logger.LogInformation("About to send a log message to {otelEndpoint}", otelEndpoint);
        var channel = GrpcChannel.ForAddress(otelEndpoint, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler()
        });

        var client = new LogsService.LogsServiceClient(channel);

        var logRecord = new LogRecord
        {
            TimeUnixNano = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1_000_000,
            SeverityText = "INFO",
            Body = new AnyValue { StringValue = "Hello from manual OTLP gRPC log sender" },
            SeverityNumber = SeverityNumber.Info
        };

        var scopeLogs = new ScopeLogs();
        scopeLogs.LogRecords.Add(logRecord);

        var resourceLogs = new ResourceLogs();
        resourceLogs.Resource = new Resource();
        resourceLogs.ScopeLogs.Add(scopeLogs);

        var request = new ExportLogsServiceRequest();
        request.ResourceLogs.Add(resourceLogs);

        var response = await client.ExportAsync(request);

        _logger.LogInformation("Export response: {response}", response);
    }
}