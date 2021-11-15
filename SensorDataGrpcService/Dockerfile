FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SensorDataGrpcService/SensorDataGrpcService.csproj", "SensorDataGrpcService/"]
RUN dotnet restore "SensorDataGrpcService/SensorDataGrpcService.csproj"
COPY . .
WORKDIR "/src/SensorDataGrpcService"
RUN dotnet build "SensorDataGrpcService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SensorDataGrpcService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SensorDataGrpcService.dll"]
