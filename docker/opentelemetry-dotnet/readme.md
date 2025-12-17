### Summary

There are three actors: The .NET service itself, OTEL collector, Prometheus and Grafana

The service pushes metrics to the otel collector in the OTEL format, accepting metrics in the endpoint "http://localhost:4318/v1/metrics"

Otel collector transforms the OTEL format metrics to Prometheus format and exposes them on "http://localhost:9464/metrics"

Prometheus scrapes the otel collector on "http://localhost:9464/metrics" periodically

Grafana scrapes the metrics from Prometheus and displays them on the dashboards; link is "http://localhost:3000/"


### Instrument the .NET app
Register the service like this:
```
services.AddOpenTelemetry()
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
```

The Nuget packages required:
```
<PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.13.1" />
<PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.13.1" />
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.13.1" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.13.0" />
<PackageReference Include="OpenTelemetry.Instrumentation.Runtime" Version="1.13.0" />
```

See `program.cs` for a quick example

Login to Grafana, create a new datasources with name `${DS_PROMETHEUS}` and put in `http://prometheus:9090` as URL