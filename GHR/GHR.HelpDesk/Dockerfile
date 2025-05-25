FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 97
EXPOSE 98

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GHR.HelpDesk/GHR.HelpDesk.csproj", "GHR.HelpDesk/"]
RUN dotnet restore "./GHR.HelpDesk/GHR.HelpDesk.csproj"
COPY . .
WORKDIR "/src/GHR.HelpDesk"
RUN dotnet build "./GHR.HelpDesk.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GHR.HelpDesk.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GHR.HelpDesk.dll"]
