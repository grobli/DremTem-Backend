FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DeviceGrpcService/DeviceGrpcService.csproj", "DeviceGrpcService/"]
RUN dotnet restore "DeviceGrpcService/DeviceGrpcService.csproj"
COPY . .
WORKDIR "/src/DeviceGrpcService"
RUN dotnet build "DeviceGrpcService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeviceGrpcService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeviceGrpcService.dll"]
