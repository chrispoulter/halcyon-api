# Halcyon Api

A .NET core api project template.

[https://halcyon-api.chrispoulter.com](https://halcyon-api.chrispoulter.com)

**Technologies used:**

-   .NET Core
    [https://github.com/dotnet/core](https://github.com/dotnet/core)

-   Entity Framework Core
    [https://github.com/dotnet/efcore](https://github.com/dotnet/efcore)

#### Custom Settings

Create `appsettings.json` file in root directory.

```
{
    "ConnectionStrings": {
        "HalcyonDatabase": ""
    },
    "Email": {
        "DropFolder": "",
        "SmtpServer": "",
        "SmtpPort": "",
        "SmtpUserName": "",
        "SmtpPassword": "",
        "NoReplyAddress": ""
    },
    "Jwt": {
        "SecurityKey": "",
        "Issuer": "",
        "Audience": "",
        "ExpiresIn": 3600
    }
}
```
