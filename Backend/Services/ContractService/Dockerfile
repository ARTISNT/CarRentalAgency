FROM mcr.microsoft.com/playwright/dotnet:v1.60.0-noble AS build

WORKDIR /src

COPY . .

RUN dotnet restore \
    Services/ContractService/ContractService.Api/ContractService.Api.csproj

RUN dotnet publish \
    Services/ContractService/ContractService.Api/ContractService.Api.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/playwright/dotnet:v1.60.0-noble

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ContractService.Api.dll"]
