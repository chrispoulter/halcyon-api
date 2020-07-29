# Halcyon

A web application template.

[https://halcyon.chrispoulter.com](https://halcyon.chrispoulter.com)

**Technologies used:**

-   .NET Core
    [https://github.com/dotnet/core](https://github.com/dotnet/core)
-   Entity Framework Core
    [https://github.com/dotnet/efcore](https://github.com/dotnet/efcore)
-   React
    [https://reactjs.org](https://reactjs.org)

#### Custom Settings

Create `appsettings.Development.json` file in web project directory.

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
        "SecurityKey": "change-me-1234567890",
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
