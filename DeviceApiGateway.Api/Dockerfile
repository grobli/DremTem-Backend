FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
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
ENTRYPOINT ["dotnet", "DeviceApiGateway.Api.dll"]
