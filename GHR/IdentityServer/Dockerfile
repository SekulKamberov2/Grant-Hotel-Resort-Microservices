# Use the .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files
COPY IdentityServer/IdentityServer.csproj IdentityServer/
COPY SharedKernel/GHR.SharedKernel.csproj SharedKernel/

# Go to the project folder before restoring
WORKDIR /src/IdentityServer
RUN dotnet restore

# Copy the rest of the source code
WORKDIR /src
COPY . .

# Build and publish the application
WORKDIR /src/IdentityServer
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Use the .NET runtime image for the final app container
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Expose port 8081 to make it accessible outside the container
EXPOSE 8081

# Copy the published app files from the build container
COPY --from=build /app/publish .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "IdentityServer.dll"]

# Optionally, you can specify the URL the application should listen to.
# For example, use the environment variable to bind the application to port 8081.
ENV ASPNETCORE_URLS="http://0.0.0.0:8081"
