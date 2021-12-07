FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["UserIdentity.Core/UserIdentity.Core.csproj", "UserIdentity.Core/"]
COPY ["DeviceManager.Core/DeviceManager.Core.csproj", "DeviceManager.Core/"]
COPY ["SensorData.Core/SensorData.Core.csproj", "SensorData.Core/"]
COPY ["DeviceApiGateway.Api/DeviceApiGateway.Api.csproj", "DeviceApiGateway.Api/"]
RUN dotnet restore "DeviceApiGateway.Api/DeviceApiGateway.Api.csproj"

COPY . .
WORKDIR "/src/DeviceApiGateway.Api"
RUN dotnet build "DeviceApiGateway.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeviceApiGateway.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS 'http://+:9090;https://+:9091'
ENV ASPNETCORE_ENVIRONMENT docker
EXPOSE 9090
EXPOSE 9091
ENTRYPOINT ["dotnet", "DeviceApiGateway.Api.dll"]
