FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore \
    Services/PaymentService/PaymentService/PaymentService.Api/PaymentService.Api.csproj

RUN dotnet publish \
    Services/PaymentService/PaymentService/PaymentService.Api/PaymentService.Api.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "PaymentService.Api.dll"]
