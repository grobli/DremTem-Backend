FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["UserIdentity.Api/UserIdentity.Api.csproj", "UserIdentity.Api/"]
COPY ["UserIdentity.Core/UserIdentity.Core.csproj", "UserIdentity.Core/"]
COPY ["UserIdentity.Data/UserIdentity.Data.csproj", "UserIdentity.Data/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "UserIdentity.Api/UserIdentity.Api.csproj"

COPY . .
WORKDIR "/src/UserIdentity.Api"
RUN dotnet build "UserIdentity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserIdentity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS 'http://+:6000;https://+:6001'
ENV ASPNETCORE_ENVIRONMENT docker
EXPOSE 6000
EXPOSE 6001
ENTRYPOINT ["dotnet", "UserIdentity.Api.dll"]
