### Ref vs. in vs. out

- ref: force function argument to be passed by reference, so changing the value inside the function changes it in the caller too

- in: same as `ref`, but difference is the argument cannot be modified. Mainly used to pass large structs to avoid copying large objects into memory (value types are copied in the stack, when passed as a function argument)

- out: used if the function needs to return multiple values
```
void GetValues(out int x, out int y)
{
    x = 10;
    y = 20;
}

int a, b;
GetValues(out a, out b);
Console.WriteLine($"{a}, {b}"); // Output: 10, 20

```
