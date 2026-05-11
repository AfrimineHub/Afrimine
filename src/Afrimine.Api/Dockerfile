# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /repo

# Copy .csproj files first for layer-cached restore
COPY ["src/Afrimine.Api/Afrimine.Api.csproj",           "src/Afrimine.Api/"]
COPY ["src/Afrimine.Shared/Afrimine.Shared.csproj",     "src/Afrimine.Shared/"]
COPY ["Afrimine.Migrations/Afrimine.Migrations.csproj", "Afrimine.Migrations/"]
COPY ["Afrimine.Model/Afrimine.Model.csproj",           "Afrimine.Model/"]
COPY ["Afrimine.Repository/Afrimine.Repository.csproj", "Afrimine.Repository/"]
COPY ["Afrimine.Services/Afrimine.Services.csproj",     "Afrimine.Services/"]

RUN dotnet restore "src/Afrimine.Api/Afrimine.Api.csproj"

# Copy all source files, preserving the repo structure
COPY src/ src/
COPY Afrimine.Migrations/ Afrimine.Migrations/
COPY Afrimine.Model/      Afrimine.Model/
COPY Afrimine.Repository/ Afrimine.Repository/
COPY Afrimine.Services/   Afrimine.Services/

WORKDIR "/repo/src/Afrimine.Api"
RUN dotnet build "Afrimine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build


# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Afrimine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Afrimine.Api.dll"]
