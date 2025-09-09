# Error handling tricks
- You can use `AppDomain.CurrentDomain.UnhandledException += OnUnhandledException` on your program.cs or startup logic, to be 100% sure that you are also handling exceptions thrown by child threads, google around for more info on this
- If you want to debug your app but logging or console.writeline just doesn't write, use this and you are gonna get the logs you need:
```
using (StreamWriter writer = new StreamWriter(@"C:\temp\someFile.txt", append: true))
                {
                    writer.WriteLine(url.ToString() + " " + response.StatusCode + response.Content.ToString());
                }
```
