# Halcyon API

A web api template.

**Technologies used:**

- .NET Core
  [https://dotnet.microsoft.com/](https://dotnet.microsoft.com/)
- PostgreSQL
  [https://www.postgresql.org/](https://www.postgresql.org/)

#### Custom Settings

Create a `appsettings.Development.json` file in the web project directory.

```
{
    "ConnectionStrings": {
        "HalcyonDatabase": "Host=localhost;Port=5432;Database=halcyon-api;Username=postgres;Password=password"
    },
    "Email": {
        "SmtpServer": "localhost",
        "SmtpPort": 1025,
        "SmtpUserName": null,
        "SmtpPassword": null,
        "NoReplyAddress": "noreply@example.com"
    },
    "Jwt": {
        "SecurityKey": "change-me-1234567890",
        "Issuer": "HalcyonApi",
        "Audience": "HalcyonClient",
        "ExpiresIn": 3600
    },
    "Seed": {
        "Users": [
            {
                "EmailAddress": "system.administrator@example.com",
                "Password": "change-me-0987654321",
                "FirstName": "System",
                "LastName": "Administrator",
                "Roles": [ "SYSTEM_ADMINISTRATOR" ]
            }
        ]
    }
}
```
