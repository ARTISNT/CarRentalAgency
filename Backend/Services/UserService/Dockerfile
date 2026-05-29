FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY UserService.Api/*.csproj UserService.Api/
COPY UserService.Application/*.csproj UserService.Application/
COPY UserService.Domain/*.csproj UserService.Domain/
COPY UserService.Infrastructure/*.csproj UserService.Infrastructure/

RUN dotnet restore UserService.Api/UserService.Api.csproj

COPY . .

RUN dotnet publish UserService.Api/UserService.Api.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "UserService.Api.dll"]
