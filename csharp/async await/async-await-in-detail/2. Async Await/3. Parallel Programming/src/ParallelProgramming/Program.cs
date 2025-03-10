using ParallelProgramming;

Console.WriteLine("Cooking Started");

var turkey = new Turkey();
var gravy = new Gravy();

// Parallel programming
// Instead of waiting for the turkey to cook before starting the gravy, we can start both at the same time
// and wait for both to finish before moving on
// We won't move on until both the turkey and the gravy are cooked

// But we need to be careful with this approach
// Deadlocks can occur if the two tasks are dependent on each other
// For example, if the turkey task is waiting for the gravy task to finish and the gravy task is waiting for the turkey task to finish
await Task.WhenAll(turkey.Cook(), gravy.Cook());

Console.WriteLine("Ready to eat");