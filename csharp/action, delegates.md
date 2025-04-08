
- How do you return functions in c#?
	 - The return type needs to have type arguments equal to types of the input arguments and the output of the function e.g.
 ```Func<typeInput1, typeInput2, typeOutput> theFunc = new function(Input1,Input2)```
 
 If the function does not have a return type, use Action instead e.g.
 ```Action<typeInput1, typeInput2> theFunc = new function(Input1,Input2)```

Returned functions are called delegates in c#

To use actions, need to pass it as a lambda function. For example, to use an action called ```assertOnlyOneValue``` that takes in two arguments, with MSTEst's ```assert.ThrowException```:
 ```Assert.ThrowsException<Exception>(() => assertOnlyOneValue(arg1, arg2));```
 i.e. NOT like this:
 ```Assert.ThrowsException<Exception>(assertOnlyOneValue(arg1, arg2));```

 