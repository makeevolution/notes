[[cloud/aws/EC2/General#^d4zZRTxc]]
- A `delegate` is a general purpose type that points to any method (i.e. pointer to a method), having a specific signature 
```
public delegate void DisplayMessage(string message);

public class Greeter
{
    private string _name;

    public Greeter(string name)
    {
        _name = name;
    }

    public void Greet(string message)
    {
        Console.WriteLine($"{_name} says: {message}");
    }
}

public class Program
{
    public static void Main()
    {
        Greeter greeter = new Greeter("Alice");

        // Assign instance method to delegate
        DisplayMessage display = greeter.Greet;

        // Invoke the delegate
        display("Hello, world!"); // Output: Alice says: Hello, world!
    }
}
```

- An `Action` is a special type of delegate, the signature is that it returns void. Using `Action` saves you from having to declare a delegate `public delegate ...`, which is the case in the example above
- A `Func` is a special type of delegate with a return value

The return type needs to have type arguments equal to types of the input arguments and the output of the function e.g.
 ```Func<typeInput1, typeInput2, typeOutput> theFunc = new function(Input1,Input2)```
 
 If the function does not have a return type, use Action instead e.g.
 ```Action<typeInput1, typeInput2> theFunc = new function(Input1,Input2)```

To use actions, need to pass it as a lambda function. For example, to use an action called ```assertOnlyOneValue``` that takes in two arguments, with MSTEst's ```assert.ThrowException```:
 ```Assert.ThrowsException<Exception>(() => assertOnlyOneValue(arg1, arg2));```
 i.e. NOT like this:
 ```Assert.ThrowsException<Exception>(assertOnlyOneValue(arg1, arg2));```

 ## When to use Invoke
- Calling a delegate directly is the same as calling `invoke` on it
```
public delegate void DisplayMessage(string message);

public class Program
{
    public static void Main()
    {
        // Assign a lambda expression to the delegate
        DisplayMessage display = (message) => Console.WriteLine(message);

        // Call the delegate using Invoke
        display.Invoke("Hello, world!"); // Output: Hello, world!

        // Alternatively, you can call the delegate directly (without Invoke)
        display("Hello, world!"); // Output: Hello, world!
    }
}
```

On WPF/WindowsForms, the `Invoke` method is embedded into the UI element. If you are in a background thread and would like to update the UI element, need to use `Invoke` instead of directly modifying the element!

```
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfAsyncAwaitWithDispatcherExample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Create a TextBlock and Button in the UI
            TextBlock textBlock = new TextBlock { Text = "Waiting...", Margin = new Thickness(10) };
            Button button = new Button { Content = "Start", Margin = new Thickness(10) };

            // Add them to a StackPanel
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(button);

            // Set the StackPanel as the content of the window
            this.Content = stackPanel;

            // Button click event
            button.Click += async (sender, e) =>
            {
                // Simulate some asynchronous work
                await Task.Run(async () =>
                {
                    await Task.Delay(2000); // Simulate a delay (e.g., long-running task)

                    // Update the TextBlock on the UI thread using Dispatcher.Invoke
                    textBlock.Dispatcher.Invoke(() =>
                    {
                        textBlock.Text = "Updated from async/await with Dispatcher!";
                    });
                });
            };
        }
    }
}
```
