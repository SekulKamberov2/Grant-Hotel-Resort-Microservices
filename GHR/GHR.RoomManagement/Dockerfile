# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base image for runtime with explicit user fallback if $APP_UID is not set
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Set a default user if APP_UID is not defined to avoid errors
ARG APP_UID=1000
USER ${APP_UID}

WORKDIR /app
EXPOSE 93
EXPOSE 94

# Build stage: restore and build the project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the project file and restore first for better caching
COPY ["GHR.RoomManagement/GHR.RoomManagement.csproj", "GHR.RoomManagement/"]
RUN dotnet restore "GHR.RoomManagement/GHR.RoomManagement.csproj"

# Copy all source files and build the project
COPY . .
WORKDIR "/src/GHR.RoomManagement"
RUN dotnet build "GHR.RoomManagement.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage: create optimized output for deployment
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GHR.RoomManagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:PublishTrimmed=false

# Final stage: runtime image with published app
FROM base AS final
WORKDIR /app

# Copy the published app from the publish stage
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GHR.RoomManagement.dll"]
