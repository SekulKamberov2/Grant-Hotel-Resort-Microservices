FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 99
 
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GHR.Rating/GHR.Rating.csproj", "GHR.Rating/"]
RUN dotnet restore "./GHR.Rating/GHR.Rating.csproj"
COPY . .
WORKDIR "/src/GHR.Rating"
RUN dotnet build "./GHR.Rating.csproj" -c $BUILD_CONFIGURATION -o /app/build
 
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GHR.Rating.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
 
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GHR.Rating.dll"]
