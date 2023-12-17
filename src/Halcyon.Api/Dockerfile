FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Halcyon.Api/Halcyon.Api.csproj", "src/Halcyon.Api/"]
RUN dotnet restore "./src/Halcyon.Api/Halcyon.Api.csproj"
COPY . .
WORKDIR "/src/src/Halcyon.Api"
RUN dotnet build "./Halcyon.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
ARG VERSION=1.0.0
RUN dotnet publish "./Halcyon.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false -p:Version="1.0.0+${VERSION}"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Halcyon.Api.dll"]