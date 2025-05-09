# Tracing and telemetry
The goal is to:
1. Make it possible to correlate log lines using System.Diagnostics.Activity and Serilog
2. Add custom context to log lines (e.g. function name, caller name)

Steps:
1. Create a class decorator (using AspectInjector library), that starts and stops an activity
```
using System.Diagnostics;
using AspectInjector.Broker;

internal static class DiagnosticsConfig
{
    public static string SourceName = "App name";

    public static ActivitySource ActivitySource { get; } = new(SourceName);
}

/*
 * A custom attribute to add telemetry to methods.
 * Any method decorated with this attribute will be wrapped in an Activity.
 * The method name will be used as the name of the Activity.
 * Serilog automatically will register the TraceId in any logging inside the method, allowing us to correlate different log messages.
 */
[Aspect(Scope.Global)]
[Injection(typeof(AddTelemetry))]
public class AddTelemetry : Attribute
{
    [Advice(Kind.Around, Targets = Target.Method)]
    public object HandleMethod(
        [Argument(Source.Name)] string methodName,
        [Argument(Source.Target)] Func<object[], object> method,
        [Argument(Source.Arguments)] object[] args)
    {
        using var activity = DiagnosticsConfig.ActivitySource.StartActivity(methodName);
        
        try
        {
            return method(args);
        }
        finally
        {
            activity?.Stop();
        }
    }
}
```
3. In `Program.cs`, start an activity listener that listens to the above. Also, configure Serilog as usual.
   - It is very important to have this listener, otherwise the activity won't work!
```
var listener = new ActivityListener
{
  ShouldListenTo = source => source.Name == DiagnosticsConfig.SourceName,
  Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
};

ActivitySource.AddActivityListener(listener);

Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(configuration)
             .CreateLogger();

builder.Logging.ClearProviders();
builder.Services.AddSerilog(Log.Logger);  // This is for if you need to log in a static class; you can do GetRequiredServices to resolve it
```
4. Use the decorator in all classes in the application; this will automatically then register an activity on method calls. Note the `Serilog.ILogger` below
```
[AddTelemetry]
internal sealed class GetMedidataCaseQueryHandler
{
    private readonly Serilog.ILogger _logger;
    public GetMedidataCaseQueryHandler(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public void Handle(){
       _logger.LogInfo("I will have trace info on me");
    }
}
```
6. Configure Serilog to also log this activity; any logging in the function will then have activity data also logged e.g. traceID. Also, add context to the log lines e.g. caller function name, etc.
```
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Serilog.Events;
using Serilog.Parsing;

/*
 * This class is used to log messages with metadata such as class name, method name, line number, and trace ID.
 * It uses Serilog for logging and provides extension methods for different log levels.
 * This makes the logs more informative and easier to trace back to the source of the log message, and correlate messages.
 */
public static class SerilogExtensions
{
    public static void LogWarning(
        this Serilog.ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        logger.LogWithMetadata(LogEventLevel.Warning, message, memberName, sourceFilePath, sourceLineNumber);
    }

    public static void LogDebug(
        this Serilog.ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        logger.LogWithMetadata(LogEventLevel.Warning, message, memberName, sourceFilePath, sourceLineNumber);
    }

    public static void LogError(
        this Serilog.ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        logger.LogWithMetadata(LogEventLevel.Error, message, memberName, sourceFilePath, sourceLineNumber);
    }

    public static void LogInfo(
        this Serilog.ILogger logger,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        logger.LogWithMetadata(LogEventLevel.Information, message, memberName, sourceFilePath, sourceLineNumber);
    }
    
    private static void LogWithMetadata(
        this Serilog.ILogger logger,
        LogEventLevel level,
        string message,
        string memberName,
        string sourceFilePath,
        int sourceLineNumber)
    {
        var className = Path.GetFileNameWithoutExtension(sourceFilePath);
        
        var templateText = "{Message}\n\t\t\t (at {Class} class in {Member} method (Line no. {Line})) \n\t\t\t";
        var template = new MessageTemplateParser().Parse(templateText);
        
        var properties = new List<LogEventProperty>
        {

            new LogEventProperty("Class", new ScalarValue(className)),
            new LogEventProperty("Member", new ScalarValue(memberName)),
            new LogEventProperty("Line", new ScalarValue(sourceLineNumber)),
            new LogEventProperty("Message", new ScalarValue(message)),
            // Any properties not in the template will be added as a LogContext, automatically appended as a { } dictionary at the end of your message
            new LogEventProperty("TraceId", new ScalarValue(Activity.Current?.TraceId.ToString())),
            // new LogEventProperty("SpanId", new ScalarValue(Activity.Current?.SpanId.ToString())),
            // new LogEventProperty("ParentId", new ScalarValue(Activity.Current?.ParentId?.ToString())),
        };

        logger.Write(new LogEvent(
            timestamp: DateTimeOffset.Now,
            level: level,
            exception: null,
            messageTemplate: template,
            properties: properties
        ));
    }
}
```

Example output:
```
[2025-05-09 19:04:32 INF] This is The Log Message
                          (at UploadCaseToArchiveService class in ExecuteAsync method (Line no. 48))
                          {TraceId="708954acbc152c61c988fe62684f24bc"}
[2025-05-09 19:04:32 INF] Another log msg
                          (at UploadCaseToArchiveService class in ExecuteAsync method (Line no. 48))
                          {TraceId="708954acbc152c61c988fe62684f24bc"}
```
