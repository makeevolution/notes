# Dotnet tricks

- Add all projects, also nested one, to a solution: `dotnet sln add (ls -r **\*.csproj)`
- Tail a one liner command forever: ` while ($true) {  aws sqs list-queues --region us-east-1; Start-Sleep -Seconds 5 } `
