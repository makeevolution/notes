# Error handling tricks
- You can use `AppDomain.CurrentDomain.UnhandledException += OnUnhandledException` on your program.cs or startup logic, to be 100% sure that you are also handling exceptions thrown by child threads

