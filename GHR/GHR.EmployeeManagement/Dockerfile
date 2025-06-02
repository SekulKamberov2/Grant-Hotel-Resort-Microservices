FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 7070
EXPOSE 7071

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["GHR.EmployeeManagement/GHR.EmployeeManagement.csproj", "GHR.EmployeeManagement/"]
RUN dotnet restore "GHR.EmployeeManagement/GHR.EmployeeManagement.csproj"

COPY . .
WORKDIR "/src/GHR.EmployeeManagement"
RUN dotnet build "GHR.EmployeeManagement.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GHR.EmployeeManagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:7070;http://+:7071

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GHR.EmployeeManagement.dll"]
