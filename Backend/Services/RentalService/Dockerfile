FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY RentalService.Api/*.csproj RentalService.Api/
COPY RentalService.Application/*.csproj RentalService.Application/
COPY RentalService.Domain/*.csproj RentalService.Domain/
COPY RentalService.Infrastructure/*.csproj RentalService.Infrastructure/

RUN dotnet restore RentalService.Api/RentalService.Api.csproj

COPY . .

RUN dotnet publish RentalService.Api/RentalService.Api.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "RentalService.Api.dll"]
