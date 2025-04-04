using UsingAsync;

Console.WriteLine("Cooking Started");

var turkey = new Turkey();

// With await keyword, now, the main thread will wait for the Cook() method to complete
// However, the main thread is free to do some other work while the Cook() method is running
// i.e. the main thread is not locked
// Just like previously, the Cook() method will be executed on another thread

// So, the difference with synchronous code is, that the main thread is not blocked to do other work
// while the Cook() method is running (i.e. it is available in the Task Parallel Library to do other work)
await turkey.Cook();

var gravy = new Gravy();
await gravy.Cook();

Console.WriteLine("Ready to eat");