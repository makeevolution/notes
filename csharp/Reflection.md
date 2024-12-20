- Use reflection using ```GetProperty()``` and ```GetValue()``` to get value of property:
```
public class Program
{
    public static void Main()
    {
        var testclass = new TESTclass();
        testclass.test = 5;
		Console.WriteLine(testclass.GetType().GetProperty("test").GetValue(testclass, null));
    }
    
    public class TESTclass
    {
        public int test { get; set; }
    }
}
```

The above works, but the following doesn't (since test is a field and not a property, use GetField() instead):

```
public class Program
{
	public static void Main()
	{
		var testclass = new TESTclass()
		Console.WriteLine(testclass.GetType().GetField("test").GetValue(testclass,null));
}
	public class TESTclass{
		public int test = 5;
	}
}
```

