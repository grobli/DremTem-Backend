FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SensorData.Api/SensorData.Api.csproj", "SensorData.Api/"]
RUN dotnet restore "SensorData.Api/SensorData.Api.csproj"
COPY . .
WORKDIR "/src/SensorData.Api"
RUN dotnet build "SensorData.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SensorData.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SensorData.Api.dll"]
