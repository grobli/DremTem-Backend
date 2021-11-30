FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["UserIdentity.Core/UserIdentity.Core.csproj", "UserIdentity.Core/"]
COPY ["DeviceManager.Core/DeviceManager.Core.csproj", "DeviceManager.Core/"]
COPY ["SensorData.Core/SensorData.Core.csproj", "SensorData.Core/"]
COPY ["ClientApiGateway.Api/ClientApiGateway.Api.csproj", "ClientApiGateway.Api/"]
RUN dotnet restore "ClientApiGateway.Api/ClientApiGateway.Api.csproj"

COPY . .
WORKDIR "/src/ClientApiGateway.Api"
RUN dotnet build "ClientApiGateway.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClientApiGateway.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS 'http://+:8080;https://+:8081'
ENV ASPNETCORE_ENVIRONMENT docker
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "ClientApiGateway.Api.dll"]
