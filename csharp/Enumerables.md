Implementing your own enumeration:

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
   