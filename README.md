# Halcyon Api

A .NET core api project template.

[https://halcyon-api.chrispoulter.com](https://halcyon-api.chrispoulter.com)

**Technologies used:**

-   .NET Core
    [https://github.com/dotnet/core](https://github.com/dotnet/core)

-   Entity Framework Core
    [https://github.com/dotnet/efcore](https://github.com/dotnet/efcore)

#### Custom Settings

Create `appsettings.development.json` file in web project directory.

```
{
    "ConnectionStrings": {
        "HalcyonDatabase": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Halcyon;Integrated Security=True;MultipleActiveResultSets=True"
    },
    "Email": {
        "SmtpServer": "smtp.mailgun.org",
        "SmtpPort": 587,
        "SmtpUserName": "",
        "SmtpPassword": "",
        "NoReplyAddress": "noreply@chrispoulter.com"
    },
    "Jwt": {
        "SecurityKey": "change-me",
        "Issuer": "HalcyonApi",
        "Audience": "HalcyonClient",
        "ExpiresIn": 3600
    },
    "Seed": {
        "EmailAddress": "",
        "Password": ""
    }
}
```
