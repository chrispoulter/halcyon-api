FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

ARG VERSION=1.0.0

COPY ["Halcyon.Web/Halcyon.Web.csproj", "Halcyon.Web/"]
RUN dotnet restore "Halcyon.Web/Halcyon.Web.csproj"
COPY . .
RUN dotnet publish "Halcyon.Web/Halcyon.Web.csproj" -c Release -o /app/publish -p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build ["/app/publish", "./"]

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "Halcyon.Web.dll"]
