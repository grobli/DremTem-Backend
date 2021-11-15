FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DeviceApiGateway/DeviceApiGateway.csproj", "DeviceApiGateway/"]
RUN dotnet restore "DeviceApiGateway/DeviceApiGateway.csproj"
COPY . .
WORKDIR "/src/DeviceApiGateway"
RUN dotnet build "DeviceApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeviceApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeviceApiGateway.dll"]
