### Summary

There are three actors: The .NET service itself, OTEL collector, Prometheus and Grafana

The service pushes metrics to the otel collector in the OTEL format, accepting metrics in the endpoint "http://localhost:4318/v1/metrics"

Otel collector transforms the OTEL format metrics to Prometheus format and exposes them on "http://localhost:9464/metrics"

Prometheus scrapes the otel collector on "http://localhost:9464/metrics" periodically

Grafana scrapes the metrics from Prometheus and displays them on the dashboards