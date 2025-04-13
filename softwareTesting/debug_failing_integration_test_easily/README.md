# How to debug API integration tests when the logs are unclear
- API integration tests is a black box test, which means:
    - Do not do any mocking of dependencies
    - Call the endpoint, gets a HTTP response, and then compares the response with expectation
- This means, we can only examine the HTTP response output
- But what if something goes wrong (e.g. exception, etc.), and the error is something that is not JSONifyable?
    - The response will be "char in pos ... is not JSONifyable
- Therefore it is unclear what fails!

- To get around this, we can either use debugger, or add temporary logging e.g. DEBUG
- But this is not possible if it fails on CI environment, or if setting up the debugger takes a long time; it's a hassle!

- So, the solution is to grab the program's logging and force it to show
- Check line 72-78 in easier to debug to see how to make the WebApplicationBuilder capture all logging (including third parties!) and show it on the console of tests