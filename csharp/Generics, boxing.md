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


- Complex example use : 

```T EnsureAvailableSimulator<T>() where T : ProxyBase, ICanSim, new();```

The part ```where T : ProxyBase, ICanSim, new()``` is called constraint. It says: The type T is a type, that fulfillls the following conditions:
(a) it is ProxyBase or drived from ProxyBase
(b) implements (aka fulfills) the interface ICanSim and
(c) has a default constructor

BTW "T" is often used as identifier for type parameters , but actually there are no restrictions for those identifiers, e.g:
```Foo EnsureAvailableSimulator<Foo>() where Foo : ProxyBase, ICanSim, new();```
is the same as our original function above.

- Another complex example use:
```
	T EnsureAvailableSimulator<T>(EquipmentTypeMinor type, params EquipmentTypeMinor[] types) where T : ICanSim; 
	
	EnsureAvailableSimulator<T>() where T : ProxyBase, ICanSim, new();
```
In the first function the type T can be every type that fulfills/implements ICanSim. e.g. ICanECGAndRR or ProSim8Proxy.

In the second function the type T must be a type of ProxyBase (as I mentioned above), e.g. VPadProxy.