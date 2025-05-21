# Use the .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the csproj files first (for caching)
COPY GHR.DutyManagement/GHR.DutyManagement.csproj GHR.DutyManagement/
COPY SharedKernel/GHR.SharedKernel.csproj SharedKernel/

# Restore dependencies
WORKDIR /src/GHR.DutyManagement
RUN dotnet restore

# Copy the full source
WORKDIR /src
COPY . .

# Build the project
WORKDIR /src/GHR.DutyManagement
RUN dotnet build -c Release -o /app/build

# Publish the app
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Use the ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GHR.DutyManagement.dll"]
