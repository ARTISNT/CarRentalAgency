FROM mcr.microsoft.com/playwright/dotnet:v1.60.0-noble AS build

WORKDIR /src

COPY . .

RUN dotnet restore \
    Services/NotificationService/NotificationService.Api/NotificationService.Api.csproj

RUN dotnet publish \
    Services/NotificationService/NotificationService.Api/NotificationService.Api.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/playwright/dotnet:v1.60.0-noble

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "NotificationService.Api.dll"]
