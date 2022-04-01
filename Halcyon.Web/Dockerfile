
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app

ARG VERSION=1.0.0
ENV App__Version=$VERSION

ARG STAGE=local
ENV App__Stage=$STAGE

ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["Halcyon.Web/Halcyon.Web.csproj", "Halcyon.Web/"]
RUN dotnet restore "Halcyon.Web/Halcyon.Web.csproj"
COPY . .
WORKDIR "/src/Halcyon.Web"
RUN dotnet build "Halcyon.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs

ARG VERSION=1.0.0
ENV REACT_APP_VERSION=$VERSION

ARG STAGE=local
ENV REACT_APP_STAGE=$STAGE

RUN npm install -g yarn
RUN dotnet publish "Halcyon.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "Halcyon.Web.dll"]
