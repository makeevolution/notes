- Null check one liner cool, this is called null propagation:

```
        if (!inputNumeric.Val?.IsNotApplicable() ?? false)
```

 IsNotApplicable() is a Philips custom extension method that returns true if the string is N/A or its variation.
 This condition is same as: 
```
 string? value = inputNumeric.Val;
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
So the LHS gives a nullable bool due to the ?, and the ? is needed because inputNumeric.Value can be null! If the value is null, then the ?? handles that and returns false
