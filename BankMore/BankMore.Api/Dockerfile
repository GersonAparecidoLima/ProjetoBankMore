FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BankMore.Api/BankMore.Api.csproj", "BankMore.Api/"]
COPY ["BankMore.Application/BankMore.Application.csproj", "BankMore.Application/"]
COPY ["BankMore.Domain/BankMore.Domain.csproj", "BankMore.Domain/"]
COPY ["BankMore.Infrastructure/BankMore.Infrastructure.csproj", "BankMore.Infrastructure/"]

RUN dotnet restore "BankMore.Api/BankMore.Api.csproj"

COPY . .
WORKDIR "/src/BankMore.Api"
RUN dotnet build "BankMore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankMore.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.Api.dll"]