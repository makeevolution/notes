// See https://aka.ms/new-console-template for more information

using CreatingTaskFromScratch;
var useAldo = true;
if (useAldo) {
    Console.WriteLine("Start simulation of our own implementation of Task class");
    Console.WriteLine("Read the code along and understand how it works");
    Console.WriteLine("Type enter to continue on every step");
    Console.ReadLine();

    Console.WriteLine("For this first one, notice that the thread IDs are different, since the AldoTask.Run method uses ThreadPool.QueueUserWorkItem, thus using a background thread");
    Console.WriteLine($"Starting Thread Id: {Thread.CurrentThread.ManagedThreadId}");
    AldoTask.Run(() => Console.WriteLine($"First AldoTask Id: {Thread.CurrentThread.ManagedThreadId}"));
    
    Console.ReadLine();

    Console.WriteLine("For this second one, notice that the (background) thread IDs that execute the two are the same, since the first action quickly finishes, and thus is available "
    + "in the thread pool as the next worker. Thus, the TPL picks that thread to execute AldoTask.ContinueWith. BUT, this is not always necessarily gonna be the case!");
    AldoTask sometask = AldoTask.Run(() => {
        Console.WriteLine($"Third AldoTask Id: {Thread.CurrentThread.ManagedThreadId}");
    });
    sometask.ContinueWith(() => Console.WriteLine($"Fourth AldoTask Id: {Thread.CurrentThread.ManagedThreadId}"));
    Console.WriteLine("Study also the docstring of ContinueWith to truly understand what's going on under the hood");
    Console.ReadLine();




}
else {
    // This is the code from the teacher, use the other else block for experimentation
    Console.WriteLine($"Starting Thread Id: {Environment.CurrentManagedThreadId}");

    await DomeTrainTask.Run(() => Console.WriteLine($"First DomeTrainTask Id: {Environment.CurrentManagedThreadId}"));

    await DomeTrainTask.Delay(TimeSpan.FromSeconds(1));

    Console.WriteLine($"Second DomeTrainTask Id: {Environment.CurrentManagedThreadId}");

    await DomeTrainTask.Delay(TimeSpan.FromSeconds(1));

    await DomeTrainTask.Run(() => Console.WriteLine($"Third DomeTrainTask Id: {Environment.CurrentManagedThreadId}"));
}