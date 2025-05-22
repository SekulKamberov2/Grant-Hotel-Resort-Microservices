FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 95
EXPOSE 96

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GHR.LeaveManagement/GHR.LeaveManagement.csproj", "GHR.LeaveManagement/"]
RUN dotnet restore "./GHR.LeaveManagement/GHR.LeaveManagement.csproj"
COPY . .
WORKDIR "/src/GHR.LeaveManagement"
RUN dotnet build "./GHR.LeaveManagement.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GHR.LeaveManagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GHR.LeaveManagement.dll"]
