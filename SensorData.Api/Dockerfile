FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SensorData.Api/SensorData.Api.csproj", "SensorData.Api/"]
COPY ["SensorData.Core/SensorData.Core.csproj", "SensorData.Core/"]
COPY ["SensorData.Data/SensorData.Data.csproj", "SensorData.Data/"]
COPY ["SensorData.Services/SensorData.Services.csproj", "SensorData.Services/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "SensorData.Api/SensorData.Api.csproj"
COPY . .
WORKDIR "/src/SensorData.Api"
RUN dotnet build "SensorData.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SensorData.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS 'http://+:7000;https://+:7001'
ENV ASPNETCORE_ENVIRONMENT docker
EXPOSE 7000
EXPOSE 7001
ENTRYPOINT ["dotnet", "SensorData.Api.dll"]
