# otelutil

Experiments with OpenTelemetry - a .NET utility for manually sending structured logs via OTLP (OpenTelemetry Protocol) using gRPC.

## Overview

This project demonstrates how to manually construct and send OpenTelemetry log records directly to an OTLP collector using gRPC, without using the high-level OpenTelemetry SDK. This is useful for understanding the low-level OTLP protocol and for situations where you need fine-grained control over the telemetry data.

## Features

- Manual construction of OTLP log records
- Direct gRPC communication with OTLP collectors
- Protocol Buffer definitions for OpenTelemetry logs
- .NET 9.0 Worker Service template

## Prerequisites

- .NET 9.0 SDK or later
- An OTLP-compatible collector running on `localhost:4317` (e.g., [Grafana Alloy](https://grafana.com/docs/alloy/latest/), OpenTelemetry Collector)

## Project Structure

```
otelutil/
├── otel-log/                           # Main application
│   ├── Program.cs                      # Application entry point
│   ├── StructuredLogSender.cs          # Core logic for sending logs via OTLP
│   ├── otel-log.csproj                 # Project file with protobuf references
│   └── opentelemetry/                  # OpenTelemetry protocol definitions
│       └── proto/                      # .proto files for logs, resources, common types
└── otelutil.sln                        # Solution file
```

## How It Works

The application:

1. Creates a gRPC channel to an OTLP endpoint (default: `http://localhost:4317`)
2. Constructs a `LogRecord` with:
   - Timestamp in Unix nanoseconds
   - Severity level (INFO)
   - Log message body
   - Severity number
3. Wraps the log record in the required OTLP structure:
   - `ScopeLogs` - Contains the log records
   - `ResourceLogs` - Contains resource metadata and scope logs
   - `ExportLogsServiceRequest` - The final request message
4. Sends the request via gRPC to the OTLP collector
5. Logs the response

## Installation & Running

### Clone the repository

```bash
git clone https://github.com/DanielBryars/otelutil.git
cd otelutil
```

### Build the project

```bash
dotnet build
```

### Run the application

```bash
cd otel-log
dotnet run
```

The application will attempt to send a log message to `http://localhost:4317`.

## Configuration

To change the OTLP endpoint, modify the `otelEndpoint` variable in `StructuredLogSender.cs`:

```csharp
var otelEndpoint = "http://localhost:4317";
```

## Dependencies

- **Microsoft.Extensions.Hosting** - Provides the worker service infrastructure
- **Grpc.Net.Client** - gRPC client for .NET
- **Google.Protobuf** - Protocol Buffers runtime
- **Grpc.Tools** - Code generation from .proto files

## Protocol Buffers

The project includes OpenTelemetry protocol definitions:

- `common.proto` - Common types used across OTLP
- `resource.proto` - Resource metadata definitions
- `logs.proto` - Log record definitions
- `logs_service.proto` - Log service RPC definitions

## Testing with a Local Collector

To test this application, you'll need an OTLP collector running locally. You can use:

### Option 1: Grafana Alloy (recommended)

```bash
docker run -p 4317:4317 grafana/alloy:latest
```

### Option 2: OpenTelemetry Collector

```yaml
# otel-collector-config.yaml
receivers:
  otlp:
    protocols:
      grpc:
        endpoint: 0.0.0.0:4317

exporters:
  logging:
    loglevel: debug

service:
  pipelines:
    logs:
      receivers: [otlp]
      exporters: [logging]
```

```bash
docker run -p 4317:4317 -v ./otel-collector-config.yaml:/etc/otel-collector-config.yaml \
  otel/opentelemetry-collector:latest --config=/etc/otel-collector-config.yaml
```

## Use Cases

This utility is useful for:

- Learning the OTLP protocol internals
- Testing OTLP collectors and endpoints
- Debugging telemetry pipelines
- Implementing custom telemetry senders where the SDK is too opinionated
- Understanding the structure of OpenTelemetry log records

## Further Reading

- [OpenTelemetry Protocol Specification](https://opentelemetry.io/docs/specs/otlp/)
- [OpenTelemetry .NET SDK](https://opentelemetry.io/docs/instrumentation/net/)
- [Protocol Buffers](https://protobuf.dev/)
