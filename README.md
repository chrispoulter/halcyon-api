# Halcyon API

A .NET Core REST API project template 👷 Built with a sense of peace and tranquillity 🙏

## Features

- .NET
  [https://dotnet.microsoft.com/](https://dotnet.microsoft.com/)
- Entity Framework
  [https://learn.microsoft.com/en-us/ef](https://learn.microsoft.com/en-us/ef)
- MassTransit
  [https://masstransit.io/](https://masstransit.io/)
- FluentValidation
  [https://fluentvalidation.net/](https://fluentvalidation.net/)
- Mapster
  [https://github.com/MapsterMapper/Mapster](https://github.com/MapsterMapper/Mapster)
- Serilog
  [https://serilog.net/](https://serilog.net/)
- Swagger
  [https://swagger.io/](https://swagger.io/)
- Docker
  [https://www.docker.com/](https://www.docker.com/)
- GitHub Actions
  [https://github.com/features/actions](https://github.com/features/actions)

## Related Projects

- Halcyon
  [https://github.com/chrispoulter/halcyon](https://github.com/chrispoulter/halcyon)

## Getting Started

### Prerequisites

- PostgreSQL
  [https://www.postgresql.org/](https://www.postgresql.org/)
- MailHog
  [https://github.com/mailhog/MailHog](https://github.com/mailhog/MailHog)

### Install dependencies

Restore NuGet packages:

```
dotnet restore
```

### Update local configuration _(optional)_

In the `src/Halcyon.Api` directory of the project, create a new `appsettings.Development.json` file. This file will override settings in `appsettings.json` during local development. This file is ignored by Git, so the secrets will not be committed to the repository.

```
{
    "ConnectionStrings": {
        "Database": "Host=localhost;Port=5432;Database=halcyon;Username=postgres;Password=password"
    },
    "Email": {
        "SmtpServer": "localhost",
        "SmtpPort": 1025,
        "SmtpUserName": null,
        "SmtpPassword": null,
        "NoReplyAddress": "noreply@example.com"
    },
    "Jwt": {
        "SecurityKey": "super_secret_key_that_should_be_changed",
        "Issuer": "HalcyonApi",
        "Audience": "HalcyonClient",
        "ExpiresIn": 3600
    },
    "Seed": {
        "Users": [
            {
                "EmailAddress": "system.administrator@example.com",
                "Password": "super_secret_password_that_should_be_changed",
                "FirstName": "System",
                "LastName": "Administrator",
                "DateOfBirth": "1970-01-01",
                "Roles": [ "SYSTEM_ADMINISTRATOR" ]
            }
        ]
    }
}
```

### Run the application

```
dotnet run --project "src/Halcyon.Api/Halcyon.Api.csproj"
```

### Access the API

Once running, you can explore the API using Swagger UI at http://localhost:5257

## Testing

This project includes unit tests. To run tests:

```
dotnet test
```

## Contributing

Feel free to submit issues or pull requests to improve the template. Ensure that you follow the coding standards and test your changes before submission.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
