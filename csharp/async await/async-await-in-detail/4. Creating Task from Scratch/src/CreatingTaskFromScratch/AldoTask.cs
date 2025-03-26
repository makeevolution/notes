using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace CreatingTaskFromScratch;

/* This class is our own implementation of the Task class, to learn
more about async await and how it works under the hood */
public class AldoTask
{
	// A task instance can be used by multiple threads.
	// Therefore, we need to use a lock to protect the state of the task.
	readonly Lock _lock = new();

	bool _completed;
	Exception? _exception;
	Action? _continueWithAction;
	ExecutionContext? _context;

	public bool IsCompleted
	{
		get
		{
			// Say thread 1 and thread 2 is assigned the same task instance.
			// Use lock so that if thread 1 sets _completed to true, thread 2 will see the updated value.
			lock (_lock)
			{
				return _completed;
			}
		}
	}

	/* The Run method is a static method that runs a given action in a background thread */
	public static AldoTask Run(Action action)
	{
		// Since this is a static method, we can instantiate AldoTask here.
		var task = new AldoTask();
		// Ask .NET to pick a worker/background thread from the thread pool
		// to run the action.
		ThreadPool.QueueUserWorkItem(_ =>
		{
			try
			{
				action();
				task.SetResult();
			}
			catch (Exception exception)
			{
				task.SetException(exception);
			}
		});
		return task;
	}

	/* This method is used to delay execution of the task, without blocking the calling thread */
	public static AldoTask Delay(TimeSpan delay)
	{
		var task = new AldoTask();
		// We use a Timer so that the calling thread is not blocked; TPL will use next available thread after the delay to execute the task.
		new Timer(_ => task.SetResult()).Change(delay, Timeout.InfiniteTimeSpan);
		return task;
	}

	/* This method is a continuation method that runs a follow-up action i.e. a callback after the task is completed 
	The first call to this method (i.e. in Program.cs) will most likely become the registration of the callback (i.e. _completed false case below)
	, but the callback can indeed be immediately executed if the main task is already completed (i.e. _completed true case below) 
	In the case of registration, once the main task is complete, the SetResult method will be called, with _continueWithAction defined.
	See CompleteTask method; if _action is defined, then you see there the callback is Invoked.

	*/
	public AldoTask ContinueWith(Action action){
		var task = new AldoTask();

		lock (_lock)
		{
			if (_completed) {

				ThreadPool.QueueUserWorkItem(_ =>
				{
					try
					{
						Console.WriteLine($"The main task is very fast and so is already completed!, executing callback {action}");
						action();
						task.SetResult();
					}
					catch (Exception exception)
					{
						task.SetException(exception);
					}
				});
			}
			else
			{
				Console.WriteLine("The main task is slow and not completed yet, so we will store the action in _continueWithAction variable and the execution context in _context variable");
				_continueWithAction = action;
				_context = ExecutionContext.Capture();
			}
		}
		return task;
	}

	/* This method is used to BLOCK the current thread until the task is completed.
	*/
	public void Wait() {
		// Wait until the task is completed.
		// A ManualResetEventSlim allows threads to wait for a signal from another thread before continuing execution. 
		// In other words, the ManualResetEventSlim class allows you to synchronize the execution of multiple threads by using set signals.
		// How does this work concretely:

		// Before we use this Wait method, logically we would've called either Run or Delay method to execute some stuff, and it will stop at the action() call (see the body of these methods)
		// Now, see the body below; while the task we called is not yet completed, here we create a ResetEventSlim and call it's Wait method.
		// But before calling it, we leverage our ContinueWith and create a callback to set the ResetEventSlim, which is then registered in _continueWithAction.
		
		// Now, when action(); in Run or Delay is complete, the next line there calls the task.SetResult() method, which calls CompleteTask private method.
		// See the CompleteTask method; there we check if _continueWithAction is not null, and if so, we run it.
		// This is indeed the case (we leveraged ContinueWith as explained above!)
		// Thus, ExecutionContext.Run will run the set ResetEventSlim callback, which will unblock the resetEventSlim's Wait method, and thus the calling thread will continue executing.
		ManualResetEventSlim? resetEventSlim = null;
		lock (_lock)
		{
			if (!_completed)
			{
				resetEventSlim = new();
				ContinueWith(() => resetEventSlim.Set());
			}
		}
		resetEventSlim?.Wait();

		if (_exception is not null)
		{
			// Use ExceptionDispatchInfo to preserve the original stack trace.
			ExceptionDispatchInfo.Throw(_exception);
		}
	}


	#region Helper Sections

	public void SetResult() => CompleteTask(null);

	public void SetException(Exception exception) => CompleteTask(exception);

	void CompleteTask(Exception? exception)
	{
		lock (_lock)
		{
			if (_completed)
			{
				throw new InvalidOperationException("Task already completed");
			}
			_exception = exception;
			_completed = true;
			if (_continueWithAction is not null)
			{
				if (_context is null)
				{
					_continueWithAction.Invoke();
				}
				else
				{
					Console.WriteLine($"running action {_continueWithAction.Method}");
					ExecutionContext.Run(_context, state => ((Action?)state)?.Invoke(), _continueWithAction);
				}
			}
		}
	}

	#endregion

	#region Awaiter Section, the magic behind the await keyword

	// We can use the await method for our custom task, if our custom task implements a method called GetAwaiter that
	// returns an struct that implements INotifyCompletion.

	public AldoTaskAwaiter GetAwaiter() => new(this);

	#endregion
}

public readonly struct AldoTaskAwaiter : INotifyCompletion
{
	readonly AldoTask _task;

	public AldoTaskAwaiter(AldoTask task) => _task = task;

	public bool IsCompleted => _task.IsCompleted;

	public void OnCompleted(Action continuation) => _task.ContinueWith(continuation);

	public void GetResult() => _task.Wait();
}