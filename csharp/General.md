- C# and Python compared https://jamescrosswell.github.io/python.html#intro
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
- The lambda function => supports auto indexing:
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

- Auto-property vs traditional getter setter: look at robotsimulator project in your github exercism https://exercism.org/tracks/csharp/exercises/robot-simulator/iterations
	- In a nutshell, a traditional getter setter needs to have a backing field.

- In c#, `properties` are those with `{ get;set;}`, while `fields` are those whose values are directly set and no getter is implemented. 
	- This is different to python, where properties and fields are basically the same thing.



