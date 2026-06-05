FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore \
    Services/RentalService/RentalService.Api/RentalService.Api.csproj

RUN dotnet publish \
    Services/RentalService/RentalService.Api/RentalService.Api.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/playwright/dotnet:v1.60.0-noble

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "RentalService.Api.dll"]
