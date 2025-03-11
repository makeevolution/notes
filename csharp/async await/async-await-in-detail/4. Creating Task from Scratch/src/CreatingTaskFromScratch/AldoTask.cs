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
	Action? _action;
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

	/* This method is a continuation method that runs a follow-up action i.e. a callback after the task is completed 
	The first call to this method (i.e. in Program.cs) will most likely become the registration of the callback (i.e. _completed false case below)
	, but the callback can indeed be immediately executed if the main task is already completed (i.e. _completed true case below) 
	In the case of registration, once the main task is complete, the SetResult method will be called, with _action defined.
	See CompleteTask method; if _action is defined, then you see there the callback is Invoked.

	*/
	public AldoTask ContinueWith(Action action){
		AldoTask task = new();

		lock (_lock)
		{
			if (_completed) {

				ThreadPool.QueueUserWorkItem(_ =>
				{
					try
					{
						Console.WriteLine("The main task is very fast and so is already completed!, executing callback");
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
				Console.WriteLine("The main task is slow and not completed yet, so we will store the action in _action variable and the execution context in _context variable");
				_action = action;
				_context = ExecutionContext.Capture();
			}
		}
		return task;
	}

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
			if (_action is not null)
			{
				if (_context is null)
				{
					_action.Invoke();
				}
				else
				{
					ExecutionContext.Run(_context, state => ((Action?)state)?.Invoke(), _action);
				}
			}
		}
	}
}