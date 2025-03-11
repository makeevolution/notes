using LifeBeforeAsync;

Console.WriteLine("Cooking Started");

var turkey = new Turkey();

// Without async keyword, the main thread will continue immediately and continue executing the next line of code.

// Another thread will be assigned to execute the Cook() method by the Task Parallel Library.
// Then, the same thread will execute the ContinueWith callback.

turkey.Cook()
	.ContinueWith(_ =>
	{
		var gravy = new Gravy();
		gravy.Cook();
	});

// block the main thread so the program doesn't end
Console.ReadLine();

