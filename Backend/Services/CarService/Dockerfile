# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Копируем csproj
COPY CarService.Api/*.csproj CarService.Api/
COPY CarService.Application/*.csproj CarService.Application/
COPY CarService.Domain/*.csproj CarService.Domain/
COPY CarService.Infrastructure/*.csproj CarService.Infrastructure/

# Restore
RUN dotnet restore CarService.Api/CarService.Api.csproj

# Копируем остальное
COPY . .

# Publish
RUN dotnet publish CarService.Api/CarService.Api.csproj \
    -c Release \
    -o /app/publish

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CarService.Api.dll"]
