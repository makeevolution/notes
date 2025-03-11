// See https://aka.ms/new-console-template for more information

using CreatingTaskFromScratch;
var useAldo = true;
if (useAldo)
{
    Console.WriteLine("Start simulation of our own implementation of Task class");
    Console.WriteLine("Read the code along and understand how it works");
    Console.WriteLine("Type enter to continue on every step");
    Console.ReadLine();

    Console.WriteLine("For this first one, we are investigating Run method.");
    Console.WriteLine("Notice that the thread IDs are different, since the AldoTask.Run method uses ThreadPool.QueueUserWorkItem, thus using a background thread");
    Console.WriteLine($"Starting Thread Id: {Thread.CurrentThread.ManagedThreadId}");
    AldoTask.Run(() => Console.WriteLine($"First AldoTask Id: {Thread.CurrentThread.ManagedThreadId}"));
    Console.WriteLine("Main thread: I can be printed before the first AldoTask finishes (not necessarily always the case; depends on speed of CPU etc.). See Wait() section below to see how to make the main thread always wait for the background to finish");

    Console.ReadLine();

    Console.WriteLine("For this second one, we are implementing ContinueWith. Study also the docstring of ContinueWith in AldoTask to truly understand what's going on under the hood");
    Console.WriteLine("Notice that the (background) thread IDs that execute the two are the same, since the first action quickly finishes, and thus is available "
    + "in the thread pool as the next worker. Thus, the TPL picks that thread to execute AldoTask.ContinueWith. BUT, this is not always necessarily gonna be the case!");
    AldoTask sometask = AldoTask.Run(() =>
    {
        Console.WriteLine($"Third AldoTask Id: {Thread.CurrentThread.ManagedThreadId}");
    });
    sometask.ContinueWith(() => Console.WriteLine($"Fourth AldoTask Id: {Thread.CurrentThread.ManagedThreadId}"));
    Console.ReadLine();

    Console.WriteLine("For this third one, we are implementing Wait. Also study the docstring of Wait in AldoTask to truly understand what's going on under the hood");
    Console.WriteLine($"Task ID before waiting with With(): {Thread.CurrentThread.ManagedThreadId}");
    var run_task = AldoTask.Run(() =>
    {
        Console.WriteLine($"Fifth AldoTask Id: {Thread.CurrentThread.ManagedThreadId}");
        Console.WriteLine("Fifth AldoTask: Will sleep for 2 seconds");
        Thread.Sleep(2000);
        Console.WriteLine("Fifth AldoTask: done sleeping");
    });
    run_task.Wait();
    Console.WriteLine($"Task ID after waiting using With(): {Thread.CurrentThread.ManagedThreadId}");
    Console.WriteLine("I will be consistently printed after the Task is done sleeping, because of Wait()");
    Console.WriteLine("The issue with using Wait() above is, the main thread is locked waiting (although not visible)! See await keyword later for a better way");
    Console.ReadLine();

    Console.WriteLine("For this fourth one, we are investigating Delay method.");
    Console.WriteLine("Why not just use Thread.Sleep?");
    Console.WriteLine("The difference between Delay and Thread.Sleep is that Delay does not block the thread, but rather schedules the continuation of the task after the delay");
    Console.WriteLine("BUT, we need to wait it.")
    Console.WriteLine("If you Wait() after the Delay task, like in example below, then it will block the thread, just like Thread.Sleep! So, the benefit is lost! Know what you are doing! Don't just use methods around!");
    Console.WriteLine("Waiting for 3 seconds, uselessly blocking the main thread with Wait() although we wanted to use Delay to purposely not block the thread");
    var delay_task = AldoTask.Delay(TimeSpan.FromSeconds(3));
    delay_task.Wait();
    Console.WriteLine("Done waiting 3 seconds, while uselessly blocking the main thread and thus achieving the same bad performance with Thread.Sleep");
    Console.WriteLine("To properly use Delay, you should use it with await keyword, which will not block the main thread. See last section though; there is a caveat!");

    Console.ReadLine();

    Console.WriteLine("For this fifth one, we are investigating await keyword. Note, there is a caveat so pay attention.");
    Console.WriteLine($"Main Thread Id: {Thread.CurrentThread.ManagedThreadId}");
    await AldoTask.Run(() => Console.WriteLine($"Sixth AldoTask Id: {Thread.CurrentThread.ManagedThreadId}"));
    Console.WriteLine($"Seventh AldoTask Id: {Thread.CurrentThread.ManagedThreadId}");
    Console.WriteLine("Although not visible, with await, the calling thread is actually free, unlike with using Wait()");

    Console.WriteLine("Notice in the above, the thread ID of the seventh is not the same as Main Thread Id, but rather the background thread's ID, while we would've expected the await to return and let the main thread do it. Why?");
    Console.WriteLine("This is the behavior of await keyword without a SynchonizationContext.");
    Console.WriteLine("In a Console App, there is no SynchronizationContext, so the await keyword will not return to the main thread, but rather continue on a background thread.");
    Console.WriteLine("But on a UI or ASP.NET Core, there is a SynchronizationContext, so the await keyword will return to the main thread, and the console writeline will be done by the main thread.");

    Console.WriteLine("Unrelatedly/interestingly, this is not the case when we use Wait() above! See the TaskID console writelines there to confirm.");

    Console.WriteLine("In Conclusion, the benefit of using await over Wait() is that the main thread is not blocked, and can be free to do other stuff. Always use it.");
    Console.WriteLine("The big caveat is that unlike Wait(), the await keyword will not always return to the main thread, but rather continue on the background thread, if there is no SynchronizationContext, like the case with Console UI here.");

    Console.WriteLine("This is the end of the simulation of our own implementation of Task class");
    Console.ReadLine();
}
else
{
    // This is the code from the teacher, use the other else block for experimentation
    Console.WriteLine($"Starting Thread Id: {Environment.CurrentManagedThreadId}");

    await DomeTrainTask.Run(() => Console.WriteLine($"First DomeTrainTask Id: {Environment.CurrentManagedThreadId}"));

    await DomeTrainTask.Delay(TimeSpan.FromSeconds(1));

    Console.WriteLine($"Second DomeTrainTask Id: {Environment.CurrentManagedThreadId}");

    await DomeTrainTask.Delay(TimeSpan.FromSeconds(1));

    await DomeTrainTask.Run(() => Console.WriteLine($"Third DomeTrainTask Id: {Environment.CurrentManagedThreadId}"));
}