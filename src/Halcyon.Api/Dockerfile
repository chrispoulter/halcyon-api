FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Halcyon.Common/Halcyon.Common.csproj", "src/Halcyon.Common/"]
COPY ["src/Halcyon.Api/Halcyon.Api.csproj", "src/Halcyon.Api/"]
RUN dotnet restore "./src/Halcyon.Api/Halcyon.Api.csproj"
COPY . .
WORKDIR "/src/src/Halcyon.Api"
RUN dotnet build "./Halcyon.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Halcyon.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Halcyon.Api.dll"]