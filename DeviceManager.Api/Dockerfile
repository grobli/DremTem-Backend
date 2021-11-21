FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
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
ENTRYPOINT ["dotnet", "DeviceManager.Api.dll"]
