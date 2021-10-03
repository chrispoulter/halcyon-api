# Halcyon

A web application template.

**Technologies used:**

- React
  [https://reactjs.org](https://reactjs.org)
- .NET Core
  [https://github.com/dotnet/core](https://github.com/dotnet/core)
- SQL Server
  [https://www.microsoft.com/sql-server](https://www.microsoft.com/sql-server)

#### Custom Settings

Create a `appsettings.Development.json` file in the web project directory.

```
{
    "ConnectionStrings": {
        "HalcyonDatabase": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Halcyon;Integrated Security=True;MultipleActiveResultSets=True"
    },
    "ApplicationInsights": {
        "ConnectionString": ""
    },
    "Email": {
        "SmtpServer": "smtp.mailgun.org",
        "SmtpPort": 587,
        "SmtpUserName": "",
        "SmtpPassword": "",
        "NoReplyAddress": "noreply@chrispoulter.com"
    },
    "Event": {
        "StorageConnectionString": "UseDevelopmentStorage=true",
        "PollingInterval": 10,
        "BatchSize": 20
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

Create a `.env` file in the web project `ClientApp` directory.

```
REACT_APP_GA_MEASUREMENT_ID=
```
