FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/WarehouseManagementService.Api/WarehouseManagementService.Api.csproj", "src/WarehouseManagementService.Api/"]
COPY ["src/WarehouseManagementService.Application/WarehouseManagementService.Application.csproj", "src/WarehouseManagementService.Application/"]
COPY ["src/WarehouseManagementService.Domain/WarehouseManagementService.Domain.csproj", "src/WarehouseManagementService.Domain/"]
COPY ["src/WarehouseManagementService.Infrastructure/WarehouseManagementService.Infrastructure.csproj", "src/WarehouseManagementService.Infrastructure/"]
RUN dotnet restore "src/WarehouseManagementService.Api/WarehouseManagementService.Api.csproj"
COPY . .
WORKDIR "/src/src/WarehouseManagementService.Api"
RUN dotnet publish "WarehouseManagementService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WarehouseManagementService.Api.dll"]
