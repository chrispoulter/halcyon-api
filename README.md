# Halcyon

A web application template.

**Technologies used:**

-   React
    [https://reactjs.org](https://reactjs.org)
-   .NET Core
    [https://github.com/dotnet/core](https://github.com/dotnet/core)
-   SQL Server
    [https://www.microsoft.com/sql-server](https://www.microsoft.com/sql-server)

#### Custom Settings

Create a `appsettings.Development.json` file in the web project directory.

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
    },
    "ApplicationInsights": {
        "ConnectionString": ""
    }
}
```
