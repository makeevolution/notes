# General

- Good explanation on deadlock in Async Await and how to mitigate https://www.youtube.com/watch?v=I4cnX_odC1M
  This is why therefore you shouldn't access Result directly like the following:
  ```
  public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            using (var reader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body))
            {
                var body = reader.ReadToEndAsync().Result;
            }
        }
  ```
  Once the executing thread hits the `await` keyword in `ReadToEndAsync`, it will return to `reader.ReadToEndAsync()` and then wait for `.Result`. But since now there is no one processing the return value of the `await`, the `reader.ReadToEndAsync().Result` will never complete, and the whole thing hangs (deadlock)

  However, this deadlock won't occur in ASP.NET Core, since in this framework, it will automatically assign a thread to process the await once the current executing thread goes back to the `reader.ReadToEndAsync().Result` line, thus no deadlock occurs! https://www.reddit.com/r/csharp/comments/130e4c6/doubt_in_chatgpt_answer_on_deadlock/

# Tips
- Try catching `Task.WhenAll` will only register the exception thrown by the first task throwing! To get all the exceptions:
  ```
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Service1 _service1;
    private readonly Service2 _service2;
    private const int sleepTime = 15;

    public Worker(ILogger<Worker> logger,
                  Service1 service1,
                  Service2 service2)
    {
        _logger = logger;
        _service1 = service1;
        _service2 = service2;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var tasks = new List<Task>
            {
                _service1.ExecuteAsync(stoppingToken),
                _service2.ExecuteAsync(stoppingToken)
            };
            try
            {
                _logger.LogInformation("Worker iteration started");
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch
            {
                LogExceptionsFromAllTasks(tasks);
            }


            _logger.LogInformation("Worker iteration ended");
            _logger.LogInformation("Sleeping for {sleepTime} seconds before the next iteration\n", sleepTime);
            await Task.Delay(sleepTime * 1000, stoppingToken);
        }
    }

    /*
     * Helper method to log all exceptions from all tasks.
     * The default behavior of Task.WhenAll is, when multiple tasks throw an exception, it will only catch the first exception!
     * We need to grab all exceptions from all tasks, so we can log them.
     */
    private void LogExceptionsFromAllTasks(List<Task> tasks)
    {
        List<Exception>? exceptions = null;

        foreach (var task in tasks)
        {
            if (task.IsFaulted)
            {
                if (task.Exception.InnerExceptions.Count > 1
                    || task.Exception.InnerException is AggregateException)
                {
                    (exceptions ??= []).AddRange(task.Exception.Flatten().InnerExceptions);
                }
                else if (task.Exception.InnerException is not null)
                {
                    (exceptions ??= []).Add(task.Exception.InnerException);
                }
            }
            else if (task.IsCanceled)
            {
                try
                {
                    // This will force the task to throw the exception if it's canceled.
                    // This will preserve all the information compared to creating a new TaskCanceledException manually.
                    task.GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    (exceptions ??= []).Add(ex);
                }
            }
        }
        
        if (exceptions?.Count > 0)
        {
            _logger.LogError("Worker iteration failed with {exceptionCount} exception(s), all exceptions:", exceptions.Count);
            foreach (var exception in exceptions)
            {
                _logger.LogError(exception, "");
            }
        }
    }
}
  ```

