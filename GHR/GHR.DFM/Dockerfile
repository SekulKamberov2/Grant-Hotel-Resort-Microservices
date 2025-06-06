FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6060
EXPOSE 6061
 
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GHR.DFM/GHR.DFM.csproj", "GHR.DFM/"]
RUN dotnet restore "./GHR.DFM/GHR.DFM.csproj"
COPY . .
WORKDIR "/src/GHR.DFM"
RUN dotnet build "./GHR.DFM.csproj" -c $BUILD_CONFIGURATION -o /app/build
 
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GHR.DFM.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
 
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GHR.DFM.dll"]
