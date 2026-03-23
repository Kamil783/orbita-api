FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Orbita.sln", "./"]
COPY ["Orbita.Api/Orbita.Api.csproj", "Orbita.Api/"]
COPY ["Orbita.Application/Orbita.Application.csproj", "Orbita.Application/"]
COPY ["Orbita.Contracts/Orbita.Contracts.csproj", "Orbita.Contracts/"]
COPY ["Orbita.Domain/Orbita.Domain.csproj", "Orbita.Domain/"]
COPY ["Orbita.Infrastructure/Orbita.Infrastructure.csproj", "Orbita.Infrastructure/"]

RUN dotnet restore "./Orbita.Api/Orbita.Api.csproj"

COPY . .
WORKDIR "/src/Orbita.Api"
RUN dotnet build "./Orbita.Api.csproj" -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/Orbita.Api"
RUN dotnet publish "./Orbita.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orbita.Api.dll"]
