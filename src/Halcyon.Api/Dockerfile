FROM mcr.microsoft.com/dotnet/aspnet:8.0.10 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0.403 AS build
WORKDIR /src
COPY . .
RUN dotnet restore

ARG VERSION=1.0.0
RUN dotnet build "./src/Halcyon.Api/Halcyon.Api.csproj" --no-restore -c Release /p:Version=${VERSION}

FROM build AS publish
RUN dotnet publish "./src/Halcyon.Api/Halcyon.Api.csproj" --no-build -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Halcyon.Api.dll"]