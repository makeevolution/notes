# Tricky C# Keywords and Common Methods for Interviews

what does this do
```
public struct Person
{
    public string Name;  // Reference type field
    public int Age;

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

public class Example
{
    public static void Main()
    {
        Person p1 = new Person("Alice", 30);
        Person p2 = p1;  // p2 gets a copy of p1, but the Name field is still a reference type

        // Modify the Name of p2
        p2.Name = "Bob";

        Console.WriteLine($"p1 Name: {p1.Name}, p1 Age: {p1.Age}");  // Alice
        Console.WriteLine($"p2 Name: {p2.Name}, p2 Age: {p2.Age}");  // Bob
    }
}
```

### **Memory / Reference Keywords**
- [`ref`](#ref)  
- [`out`](#out)  
- [`in`](#in)  
- [`readonly`](#readonly)  
- [`const`](#const)  

---

### **Asynchronous Programming**
- [`async`](#async)  
- [`await`](#await)  
- [`ConfigureAwait(false)`](#configureawaitfalse)  

---

### **Type Control**
- [`var`](#var)  
- [`dynamic`](#dynamic)  
- [`object`](#object)  
- [`default`](#default)  

---

### **Code Behavior / Modifiers**
- [`virtual`](#virtual)  
- [`override`](#override)  
- [`new`](#new)  
- [`abstract`](#abstract)  
- [`sealed`](#sealed)  
- [`partial`](#partial)  
- [`unsafe`](#unsafe)  
- [`fixed`](#fixed)  
- [`stackalloc`](#stackalloc)  

---

### **Threading / Safety**
- [`lock`](#lock)  
- [`volatile`](#volatile)  

---

### **Code Generation / Syntax Sugar**
- [`yield`](#yield)  
- [`nameof`](#nameof)  
- [`using`](#using)  
- [`is`](#is) / [`as`](#as)  
- [`switch` (expression)](#switch-expression)  

---

### **Method-related Keywords**
- [`delegate`](#delegate)  
- [`event`](#event)  
- [`add`](#add)  
- [`remove`](#remove)  
- [`get`](#get)  
- [`set`](#set)  
- [`init`](#init)  
- [`async` (in method context)](#async-in-method-context)  
- [`await` (in method context)](#await-in-method-context)  

---

### **Commonly Used Methods**
- [`Wait()`](#wait)  
- [`Result`](#result)  
- [`ToString()`](#tostring)  
- [`Dispose()`](#dispose)  
- [`Clone()`](#clone)  
- [`Equals()`](#equals)  
- [`GetHashCode()`](#gethashcode)  
- [`CompareTo()`](#comparerto)  
- [`Contains()`](#contains)  
- [`Substring()`](#substring)  
- [`Add()`](#add)  
- [`Remove()`](#remove)  

---

## Explanations

### **`ref`**
Passes a parameter by reference, meaning the called method can modify the variable. The variable must be initialized before being passed.

### **`out`**
Similar to `ref`, but the parameter doesn't need to be initialized by the caller. The method must assign a value before returning.

### **`in`**
Passes a parameter by reference but ensures it's read-only in the method (improves performance when passing large structs).

### **`readonly`**
Defines a field that can only be assigned during its declaration or in the constructor of the class it belongs to.

### **`const`**
Declares a constant field or local variable whose value is set at compile time and cannot be changed.

---

### **`async`**
Marks a method as asynchronous. The method will return a `Task` or `Task<T>` and can contain the `await` keyword.

### **`await`**
Pauses the method's execution until the awaited task completes. It is used only inside `async` methods.

### **`ConfigureAwait(false)`**
Used with `await` to prevent capturing the synchronization context, which is useful in non-UI applications to avoid unnecessary context switching.

---

### **`var`**
Used for implicit typing. The type is inferred by the compiler based on the right-hand side of the assignment.

### **`dynamic`**
Allows for dynamic typing, where type checking is deferred until runtime. This can lead to runtime errors if the type is incorrect.

### **`object`**
The base type from which all other types (except `null`) derive. Every type in C# is an `object`.

### **`default`**
Returns the default value for a type (e.g., `0` for numeric types, `null` for reference types).

---

### **`virtual`**
Allows a method or property to be overridden by a derived class.

### **`override`**
Provides a new implementation for a method or property that was marked as `virtual` or `abstract` in the base class.

### **`new`**
Hides a base class member. It's not polymorphic (unlike `override`), and the member is called based on the compile-time type of the reference.

### **`abstract`**
Defines a method or class that cannot be instantiated directly and must be implemented or inherited by a derived class.

### **`sealed`**
Prevents further inheritance of a class or method overriding.

### **`partial`**
Allows the definition of a class, struct, or interface to be split across multiple files.

### **`unsafe`**
Enables low-level memory operations, such as pointers, by marking a block of code as unsafe.

### **`fixed`**
Used in `unsafe` code to pin a variable in memory so it isn't moved by the garbage collector.

### **`stackalloc`**
Allocates memory on the stack for a block of memory, typically used with `Span<T>`.

---

### **`lock`**
Provides a mechanism to prevent multiple threads from executing a block of code simultaneously. It ensures thread safety by locking a critical section.

### **`volatile`**
Indicates that a field's value can be changed by multiple threads and ensures the most up-to-date value is read from memory.

---

### **`yield`**
Used in an iterator method to return a sequence of values lazily. The method doesn’t need to allocate a collection.

### **`nameof`**
Returns the string name of a variable, method, or property, and is typically used in exceptions or logging.

### **`using`**
Ensures that an `IDisposable` object is disposed of as soon as it goes out of scope, releasing resources like file handles or database connections.

### **`is` / `as`**
`is` checks if an object is of a specific type, and `as` attempts to cast an object to a specified type (returning `null` if the cast fails).

### **`switch` (expression)**
A more concise version of the traditional `switch` statement, allowing a value to be returned from the `switch` itself.

---

### **Method-related Keywords**

### **`delegate`**
Defines a type that represents references to methods with a particular parameter list and return type. It's used for implementing event handlers and callbacks.

### **`event`**
Used to declare an event in a class or interface, which is a type of `delegate`. Events provide a way for other objects to be notified when something happens.

### **`add`**
Defines a method to add a delegate to an event. This is part of the event's custom accessor methods.

### **`remove`**
Defines a method to remove a delegate from an event. Like `add`, it's part of the event's custom accessor methods.

### **`get`**
Defines the getter method of a property, which retrieves the property value.

### **`set`**
Defines the setter method of a property, which assigns a value to the property.

### **`init`**
Used to define a property that can only be set during object initialization (only in C# 9+). This provides a way to have immutable objects with setter logic at initialization time.

### **`async` (in method context)**
Marks a method as asynchronous, similar to the earlier explanation, but focusing on its specific use in method declarations.

### **`await` (in method context)**
Pauses execution in an `async` method until the awaited task completes, allowing asynchronous operations to execute without blocking the calling thread.

---

### **Commonly Used Methods**

### **`Wait()`**
Blocks the calling thread until the asynchronous operation has completed. Used with `Task` or `Task<T>` to block until the operation is done.

### **`Result`**
A property of `Task<T>` that gets the result of an asynchronous operation. It blocks the thread and waits for the task to complete.

### **`ToString()`**
Returns a string representation of the object. Every type in C# (including `null`) has this method, and it's often overridden to provide meaningful descriptions.

### **`Dispose()`**
Releases unmanaged resources used by an object. Typically used in classes that implement `IDisposable`.

### **`Clone()`**
Creates a shallow copy of the object. This is generally used when you want a copy of an object without affecting the original object’s state.

### **`Equals()`**
Compares an object with another to check if they are equal. It can be overridden to provide custom comparison logic.

### **`GetHashCode()`**
Returns a hash code for the object. This is used in hashing algorithms and collections such as `Dictionary`.

### **`CompareTo()`**
Compares the current object with another object of the same type and returns an integer that indicates whether the current object is less than, equal to, or greater than the other.

### **`Contains()`**
Checks if a sequence or collection contains a specific element. Commonly used with strings and collections.

### **`Substring()`**
Returns a substring from a string, starting at a specified index and optionally with a specified length.

### **`Add()`**
Adds an element to a collection, such as a `List`, `Dictionary`, or `Queue`.

### **`Remove()`**
Removes the first occurrence of a specified element from a collection or string.

---

Let me know if you need further modifications or additions!