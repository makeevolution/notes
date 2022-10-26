# C# lessons learned

- Some good C# quizzes: 
https://www.tutorialsteacher.com/online-test/csharp-test3
- When making a setter, use the reserved keyword "value" to refer to the input
- Sometimes people make default argument input in C# as shown (instead of the usual ```public Ball(string ballType = "regular"))``` :
```
public class Ball {
  public string ballType { get; set; }
  
  public Ball(string ballType) {
     this.ballType = ballType;
  }
  // The this refers to the current instance
  public Ball(): this("regular"){}
}
```
- In C#, ```where``` method is the same as ```filter``` method of Python 
- The lambda function => has a cool feature:
  ```
  public static int Distance(string firstStrand, string secondStrand)
    {
        if (firstStrand.Length != secondStrand.Length)
        {
            throw new ArgumentException();
        }
        return firstStrand.Where((nucleotide1, i) => nucleotide1 != secondStrand[i]).Count();
    }
	```
	
The lambda function above takes in a tuple, with ```nucleotide1``` being each character and i being the index. The => function makes the index automatically! https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions (entry right above the type inference in lambda exp.) 
https://exercism.io/tracks/csharp/exercises/hamming/solutions/e66758927b84447a8d7825f09fed0179

- ```Select()``` method is same as Python's ```map()``` function

- Use ```Aggregate``` method (or ```reduce``` in python) to cumulatively do something on the collection. For example:
   ```
    private static Dictionary<char, int> NucleotideDict = new Dictionary<char, int>()
        {
            {'A', 0},
            {'C', 0},
            {'G', 0},
            {'T', 10}
        };

	
    public static IDictionary<char, int> Count(string sequence) =>
    sequence.Aggregate(NucleotideDict,
					 (dict, letter) =>
					 {
			         if (dict.ContainsKey(letter)) dict[letter]++;
         else throw new ArgumentException($"Invalid symbol '{letter}' in \"{sequence}\".");
         return dict;
								     },
				        dict => dict);
	```

The ```Count``` method above is supposed to take in a sequence of strings, and return a copy of NucleotideDict, updated with how many times each letter in the Dictionary occurs in the string.

In the first run of aggregation, dict in the above equals NucleotideDict, and performs the operation with the first letter in the sequence, which returns the dict updated. 

Then on the second run, dict in the above then equals the updated dict, and is operated with second letter in sequence, and so on and so forth. 

After all runs are done, the third argument (dict=>dict) specifies what part of the aggregation is to be returned. In this case we return everything.

- What to do if you need to have a Main method on your class but also have an XUnit test to test the class
https://stackoverflow.com/questions/42372342/xunit-test-project-with-only-1-main-method-program-has-more-than-one-entry-poi

- Auto-property vs traditional getter setter: look at robotsimulator project in your github exercism https://exercism.org/tracks/csharp/exercises/robot-simulator/iterations
	In a nutshell, a traditional getter setter needs to have a backing field.

- In c#, properties are those with { get;set;}, while fields are those whose values are directly set and no getter is implemented. This is different to python, where properties and fields are basically the same thing.

- Use reflection using ```GetProperty()``` and ```GetValue()``` to get value of property:
```
public class Program
{
	public static void Main()
	{
		var testclass = new TESTclass();
		testclass.test = 5;
		Console.WriteLine(testclass.GetType().GetProperty("test").GetValue(testclass,null));
	}
	public class TESTclass{
		public int test {get; set;}
	}
}
```

The above works, but the following doesn't (since test is a field and not a property, use GetField() instead):

```
public class Program
{
	public static void Main()
	{
		var testclass = new TESTclass();
		Console.WriteLine(testclass.GetType().GetProperty("test").GetValue(testclass,null));
	}
	public class TESTclass{
		public int test = 5;
	}
}
```

-  Generic vs. Non Generic collections.
  
  Collections store a series of values or objects.
  
  Generic collections hold elements of same datatypes.
  
  Example:
  ```List<T>```, a list of elements of type T, where T can be a custom class or just int/string/anything else. But all elements have to be of type T
  
   Non generic collections hold elements of different datatypes, and you don't have to specify the type of the elements it holds.
   
   Example:
   ArrayList, HashTables, etc.

   More on this here https://dotnettutorials.net/lesson/advantages-and-disadvantages-of-collection/
   
   The general consensus is to use Generic collections always. It was added in dotnet framework to handle problems with non-generic collections, where since each element is boxed into object data type, retreiving the values from the collection is slow.

- Implementing your own enumeration:

 To iterate through elements defined in a class, the class needs to implement the IEnumerable interface, which implements GetEnumerator() method. In our class, we need to define the functionality of the GetEnumerator() method. Inside this method, define what the enumerator must do. 
 
Now, when the class is called from an iterator (e.g. foreach), the iterator will  call the class's GetEnumerator() method and implement the functionality defined in there.

  If you are using the inenumerable<T\> interface instead of the inenumerable (i.e. using generic collections), then you need to insert also the method:
  ```
  IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
```
  since IEnumerable<T\> implements IEnumerable, thus you must define both the non generic implementation (i.e. the custom GetEnumerator method you made) and the generic implementation (i.e. the code above)
  
   More info:
   https://www.youtube.com/watch?v=21dQjXVz8cc
   https://rextester.com/discussion/YEAT49581/IEnumerable-T-generic-interface-implementation-
   
- Using main in XUnit
 
If you use XUnit and want to debug the program from main, but have the problem "Program has more than one entry point defined", then do the following:
https://andrewlock.net/fixing-the-error-program-has-more-than-one-entry-point-defined-for-console-apps-containing-xunit-tests/
add <GenerateProgramFile>false</GenerateProgramFile> to propertygroup of your csproj file.
  
- <\> complex example in c# : 

First example:

```T EnsureAvailableSimulator<T>() where T : ProxyBase, ICanSim, new();```

The part ```where T : ProxyBase, ICanSim, new()``` is called constraint. It says: The type T is a type, that fulfillls the following conditions:
(a) it is ProxyBase or drived from ProxyBase
(b) implements (aka fulfills) the interface ICanSim and
(c) has a default constructor

BTW "T" is often used as identifier for type parameters , but actually there are no restrictions for those identifiers, e.g:
```Foo EnsureAvailableSimulator<Foo>() where Foo : T : ProxyBase, ICanSim, new();```
is actually the same as our original function above.

Another example:

    T EnsureAvailableSimulator<T>(EquipmentTypeMinor type, params EquipmentTypeMinor[] types) where T : ICanSim;

    T EnsureAvailableSimulator<T>() where T : ProxyBase, ICanSim, new();

In the first function the type T can be every type that fulfills/implements ICanSim. e.g. ICanECGAndRR or ProSim8Proxy.

In the second function the type T must be a type of ProxyBase (as I mentioned above), e.g. VPadProxy.

- use cw for print to console shorthand

- to allow user input, change launch.json console from internalConsole to integratedTerminal

- ctrl x to remove line
- ctrl shift enter to add line
- ctrl shift p to open nuget gallery
- ctrl k c visual studio bulk comment, ctrl k u uncomment

- Null check one liner cool, this is called null propagation:

```
        if (!inputNumeric.Val?.IsNotApplicable() ?? false)
```

 IsNotApplicable() is a Philips custom extension method that returns true if the string is N/A or its variation.
        The condition is same as:
        
```
 string? value = inputNum.Val;
 bool? isNotApplicable; //Nullable bool
 if (value != null)
 {
    isNotApplicable = SomeExtensionClass.IsNotApplicable(value);
 }
 //else
 {
    isNotApplicable = null;
 }
 bool result;
 if (isNotApplicable != null)
 {
    result = isNotApplicable;
 }
 else
 {
    result = false;
 }
 result = !result;
```

 So the LHS gives a nullable bool due to the ?, and 
 	   the ? is needed because inputNumeric.Value can
 		be null!
                 
- How to return functions in c#:

The return type neeeds to have type arguments equal to types of the input arguments and the output of the function e.g.
 ```Func<typeInput1, typeInput2, typeOutput) theFunc = new function(Input1,Input2)```
 
 If the function does not have a return type, use Action instead e.g.
 ```Action<typeInput1, typeInput2) theFunc = new function(Input1,Input2)```

Returned functions are called delegates in c#

 To use actions, need to pass it as a lambda function. For example, to use an action called ```assertOnlyOneValue``` that takes in two arguments, with MSTEst's ```assert.ThrowException```:
 ```Assert.ThrowsException<Exception>(() => assertOnlyOneValue(arg1, arg2));```
 i.e. NOT like this:
 ```Assert.ThrowsException<Exception>(assertOnlyOneValue(arg1, arg2));```

 - Unit testing 
 
 Learnt from Philips, it has 3 steps: Arrange, act, assert. Arrange step prepares everything, act step combines all the prepared stuff and then obtains the thing to be tested, arrange step tests/checks if the thing is as desired or does what it is supposed to do.

- String format specifier
To format a string to e.g. 2 digits always, etc. look here [Standard numeric format strings | Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?redirectedfrom=MSDN)


