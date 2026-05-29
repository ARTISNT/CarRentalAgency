FROM mcr.microsoft.com/playwright/dotnet:v1.60.0-noble AS build

WORKDIR /src

COPY ContractService.Api/*.csproj ContractService.Api/
COPY ContractService.Application/*.csproj ContractService.Application/
COPY ContractService.Domain/*.csproj ContractService.Domain/
COPY ContractService.Infrastructure/*.csproj ContractService.Infrastructure/


RUN mkdir -p /data/contracts 

RUN dotnet restore ContractService.Api/ContractService.Api.csproj

COPY . .

RUN dotnet publish ContractService.Api/ContractService.Api.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/playwright/dotnet:v1.60.0-noble

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ContractService.Api.dll"]
