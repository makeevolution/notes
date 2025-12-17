
// Example instrumenting
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
        .WithMetrics(metrics =>
        {
            metrics
                .SetResourceBuilder(ResourceBuilder
                                    .CreateDefault()
                                    .AddService("MyCoolService"))
                .AddRuntimeInstrumentation() // GC, JIT, exceptions…
                .AddOtlpExporter(opt =>
            {
                
                opt.Endpoint = new Uri("http://localhost:4318/v1/metrics");
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;
            });
        });
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/throw", () =>
{
    throw new Exception("This is a test exception for OpenTelemetry!");
});

app.Run();
