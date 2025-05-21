FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY OcelotApiGateway/OcelotApiGateway.csproj OcelotApiGateway/

# Ensure correct working directory for restore
WORKDIR /src/OcelotApiGateway
RUN dotnet restore

WORKDIR /src
COPY . .

WORKDIR /src/OcelotApiGateway
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OcelotApiGateway.dll"]
