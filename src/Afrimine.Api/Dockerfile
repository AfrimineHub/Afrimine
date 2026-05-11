# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Afrimine.Api/Afrimine.Api.csproj", "Afrimine.Api/"]
COPY ["Afrimine.Migrations/Afrimine.Migrations.csproj", "Afrimine.Migrations/"]
COPY ["Afrimine.Model/Afrimine.Model.csproj", "Afrimine.Model/"]
COPY ["Afrimine.Repository/Afrimine.Repository.csproj", "Afrimine.Repository/"]
COPY ["Afrimine.Services/Afrimine.Services.csproj", "Afrimine.Services/"]
COPY ["src/Afrimine.Shared/Afrimine.Shared.csproj", "Afrimine.Shared/"]

RUN dotnet restore "Afrimine.Api/Afrimine.Api.csproj"
COPY src/ .
WORKDIR "/src/Afrimine.Api"
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