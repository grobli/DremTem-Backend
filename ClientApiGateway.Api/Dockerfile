FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
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
ENTRYPOINT ["dotnet", "ClientApiGateway.Api.dll"]
