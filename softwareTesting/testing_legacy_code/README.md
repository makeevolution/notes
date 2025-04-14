# Snapshot testing

## Intro
- If we have legacy code and would like to improve, we need to write tests for it before refactoring to make sure existing functionality is not broken
- One way is to use `snapshot testing` using `Verify`, see the test for example on how
- Also look at the csproj to make sure you also understand the dependencies required!

## How it works
- When you run a `Verify` test, it will look for the file with `*TestFunctionName.verified.*`. 
- If it doesn't exist or if the contents are not as expected, it will generate `*TestFunctionName.received.*`, which is what the output is
- You can then examine the contents, and if you are sure it is what you want, change the filename to `*TestFunctionName.verified.*`
- Thus now you have a test with an expectation file ensuring current behavior is maintained, and can refactor safely.