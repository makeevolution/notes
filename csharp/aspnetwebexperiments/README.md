# Experiment with various web .NET services
## Important things to register in a web .NET service

## How to add data protection (ask what this is for)
`services.AddDataProtection().PersistKeysToFileSystem`

## How to show detailed error pages in development environment
 ``` 
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
app.UseDeveloperExceptionPage();
}
else
{
app.UseExceptionHandler("/Home/Error");
}
```

## How to create a CSP for dotnet
https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/CSP 
For .NET use NWebSec library


## How to avoid user enumeration attacks
User enumeration attacks happen when an attacker can determine valid usernames or email addresses in your system by observing the system's responses to login or registration attempts. To prevent this, you can implement the following strategies:

When a login API responds faster for non-existent users than for existing users, attackers can use response time differences to figure out which usernames exist.

For example:

```
Username	Response Time
Alice	120 ms
Bob	50 ms
```
An attacker notices “Bob” responds faster → maybe Bob doesn’t exist → username enumeration.

The goal is to make all login attempts take roughly the same time, regardless of whether the username exists.

```
private static async Task AvoidUserEnumeration(DateTime startTime)
{
var alreadySpent = (DateTime.UtcNow - startTime).TotalMilliseconds;
if (alreadySpent > AuthCallRandomWaitMaxMilliSeconds)
return;

    await Task.Delay(AuthCallRandomWaitMaxMilliSeconds - (int)alreadySpent);
}
```
Step by Step

Track start time

startTime is the time the authentication logic started.

You calculate how long the processing has already taken:

`var alreadySpent = (DateTime.UtcNow - startTime).TotalMilliseconds;`


Check if we need to wait

If the elapsed time is already more than the configured max, just return:

`if (alreadySpent > AuthCallRandomWaitMaxMilliSeconds) return;`


Delay for the remaining time

Otherwise, delay for the difference so that the total response time is roughly AuthCallRandomWaitMaxMilliSeconds:

`await Task.Delay(AuthCallRandomWaitMaxMilliSeconds - (int)alreadySpent);`
