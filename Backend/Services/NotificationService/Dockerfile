FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY NotificationService.Api/*.csproj NotificationService.Api/
COPY NotificationService.Application/*.csproj NotificationService.Application/
COPY NotificationService.Domain/*.csproj NotificationService.Domain/
COPY NotificationService.Infrastructure/*.csproj NotificationService.Infrastructure/

RUN dotnet restore NotificationService.Api/NotificationService.Api.csproj

COPY . .

RUN dotnet publish NotificationService.Api/NotificationService.Api.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "NotificationService.Api.dll"]
