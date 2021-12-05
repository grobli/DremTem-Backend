FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["UserIdentity.Core/UserIdentity.Core.csproj", "UserIdentity.Core/"]

COPY ["DeviceManager.Core/DeviceManager.Core.csproj", "DeviceManager.Core/"]
COPY ["DeviceManager.Data/DeviceManager.Data.csproj", "DeviceManager.Data/"]
COPY ["DeviceManager.Services/DeviceManager.Services.csproj", "DeviceManager.Services/"]
COPY ["DeviceManager.Api/DeviceManager.Api.csproj", "DeviceManager.Api/"]
RUN dotnet restore "DeviceManager.Api/DeviceManager.Api.csproj"
COPY . .
WORKDIR "/src/DeviceManager.Api"
RUN dotnet build "DeviceManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeviceManager.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS 'http://+:5000;https://+:5001'
ENV ASPNETCORE_ENVIRONMENT docker
EXPOSE 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "DeviceManager.Api.dll"]
