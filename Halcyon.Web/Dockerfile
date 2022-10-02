FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS base
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs
RUN npm install -g yarn

FROM base AS build
WORKDIR /src

ARG VERSION=1.0.0
ENV VITE_VERSION=$VERSION

COPY ["Halcyon.Web/Halcyon.Web.csproj", "Halcyon.Web/"]
RUN dotnet restore "Halcyon.Web/Halcyon.Web.csproj"
COPY . .
RUN dotnet publish "Halcyon.Web/Halcyon.Web.csproj" -c Release -o /app/publish -p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS final
WORKDIR /app
COPY --from=build ["/app/publish", "./"]

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "Halcyon.Web.dll"]
