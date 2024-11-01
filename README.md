# Basic .NET 8 Console App

Yes, there are much less verbose starting projects. No, you do not need all this stuff to play with .NET.

Quickly clone this repository to get a .NET 8 console app which:

1. Has dependency injection usage examples ([DemoService](./ConsoleApp/Services/DemoService.cs))
1. Docker container support
1. Has GitHub actions to build a container
1. Example getting AccessToken from Azure AD following client credentials flow (for daemons)
    * Built-in example assumes Azure App Registration has `Microsoft Graph : User.Read.All` application permission granted.
1. devcontainer configured - only need container runtime, not dotnet locally installed (TODO)

## Build & Run Docker container

```bash
cd BasicConsole/
docker build -t dotnet-basic-console:latest .
docker run --rm dotnet-basic-console:latest
```