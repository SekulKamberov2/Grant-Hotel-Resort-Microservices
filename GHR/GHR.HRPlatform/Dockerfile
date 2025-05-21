# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8082

# Tell ASP.NET Core to listen on this port inside the container
ENV ASPNETCORE_URLS=http://0.0.0.0:8082

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GHR.HRPlatform/GHR.HRPlatform.csproj", "GHR.HRPlatform/"]
RUN dotnet restore "./GHR.HRPlatform/GHR.HRPlatform.csproj"
COPY . .
WORKDIR "/src/GHR.HRPlatform"
RUN dotnet build "./GHR.HRPlatform.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GHR.HRPlatform.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image for running
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GHR.HRPlatform.dll"]
