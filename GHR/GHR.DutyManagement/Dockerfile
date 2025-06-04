FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY GHR.DutyManagement/GHR.DutyManagement.csproj GHR.DutyManagement/
COPY SharedKernel/GHR.SharedKernel.csproj SharedKernel/

WORKDIR /src/GHR.DutyManagement
RUN dotnet restore

WORKDIR /src
COPY . .

WORKDIR /src/GHR.DutyManagement
RUN dotnet build -c Release -o /app/build

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GHR.DutyManagement.dll"]
