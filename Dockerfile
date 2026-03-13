# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AuthService.sln ./
COPY AuthService.Host/AuthService.Host.csproj AuthService.Host/
COPY AuthService.Application/AuthService.Application.csproj AuthService.Application/
COPY AuthService.Domain/AuthService.Domain.csproj AuthService.Domain/
COPY AuthService.Infrastructure/AuthService.Infrastructure.csproj AuthService.Infrastructure/
COPY Auth.Contracts/Auth.Contracts.csproj Auth.Contracts/
COPY AuthService.Shared/AuthService.Shared.csproj AuthService.Shared/

RUN dotnet restore AuthService.sln

COPY . .
RUN dotnet publish AuthService.Host/AuthService.Host.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.Host.dll"]
