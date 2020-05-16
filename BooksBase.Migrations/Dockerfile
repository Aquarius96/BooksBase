#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BooksBase.Migrations/BooksBase.Migrations.csproj", "BooksBase.Migrations/"]
RUN dotnet restore "BooksBase.Migrations/BooksBase.Migrations.csproj"
COPY . .
WORKDIR "/src/BooksBase.Migrations"
RUN dotnet build "BooksBase.Migrations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksBase.Migrations.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksBase.Migrations.dll"]