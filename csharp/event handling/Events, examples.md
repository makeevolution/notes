- Simple:
```
public class WorkManager
{
    public event EventHandler? WorkCompleted;

    public void DoWork()
    {
        Console.WriteLine("👷 Doing work...");

        // Simulate some sync work
        Thread.Sleep(1000);

        // Raise the event using Invoke
        WorkCompleted?.Invoke(this, EventArgs.Empty);
    }
}

public class Logger
{
    public void OnWorkCompleted(object? sender, EventArgs e)
    {
        Console.WriteLine("✅ Logger: Work has been completed!");
    }
}

class Program
{
    static void Main()
    {
        var manager = new WorkManager();
        var logger = new Logger();

        // Subscribe to the event
        manager.WorkCompleted += logger.OnWorkCompleted;

        // Run the work, and raise the WorkCompletedEvent (see the function body)
        // When it hits the WorkCompleted?.Invoke() line, this raises the WorkCompleted event, and all subscribers to this event (i.e. for this example, logger.OnWorkCompleted) will be run; if there are multiple subscribers, they shall be run one by one.
        // Problem with this is that the call is synchronous, meaning that the calling thread will be blocked until all
        // subscribers finish; if a subscriber is doing Thread.Sleep(5000), then calling thread is sitting idle.

        manager.DoWork();

        // Output:
        // 👷 Doing work...
        // ✅ Logger: Work has been completed!
    }
}
```






- More robust example:
```
public class MyEventSource
{
    // Instead of EventHandler, we define the event as a Func for async support
    public event Func<object, CancellationToken, Task>? OnWorkRequested;

    // Method to raise the event
    public async Task RaiseWorkRequestedAsync(CancellationToken cancellationToken)
    {
	    // Different to the simple example above, where to raise the event we called
	    // OnWorkRequested?.Invoke(this, EventArgs.Empty),
	    // notice we defined the eventhandlers for the event as a Func (see the property), and here we call the method GetInvocationList instead.
	    // Defining the handler as a Func means we can  use await each handler since we define the return value as Task i.e. calling thread is then freed! Additionally, since we use await, don't forget to send in robustness features e.g. CancellationToken, so if main app shuts down, the handlers are also cancelled (since they run on a separate thread)
        var handlers = OnWorkRequested?.GetInvocationList();
        if (handlers == null) return;

        foreach (Func<object, CancellationToken, Task> handler in handlers)
        {
            try
            {
	            // handler here will be HandleWorkAsync; we registered this in MyBackgroundService below
                await handler(this, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Handler {handler.Method.Name} failed: {ex.Message}");
            }
        }
    }
}



public class MyBackgroundService : BackgroundService
{
    private readonly MyEventSource _eventSource = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Register an event handler to the event
        _eventSource.OnWorkRequested += HandleWorkAsync;

        // Raise events periodically
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
            // Trigger code that internally will raise the event
            await _eventSource.RaiseWorkRequestedAsync(stoppingToken);
        }

        // Clean up
        _eventSource.OnWorkRequested -= HandleWorkAsync;
    }

    private async Task HandleWorkAsync(object sender, CancellationToken token)
    {
        Console.WriteLine("👷 Handling background work...");
        await Task.Delay(1000, token); // Simulate work
    }
}
```
