#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Web/Web.csproj", "Web/"]
COPY ["BooksBase.DataAccess/BooksBase.DataAccess.csproj", "BooksBase.DataAccess/"]
COPY ["BooksBase.Models/BooksBase.Models.csproj", "BooksBase.Models/"]
COPY ["BooksBase.CoreFunctions/BooksBase.CoreFunctions.csproj", "BooksBase.CoreFunctions/"]
COPY ["BooksBase.Infrastructure/BooksBase.Infrastructure.csproj", "BooksBase.Infrastructure/"]
COPY ["BooksBase.Migrations/BooksBase.Migrations.csproj", "BooksBase.Migrations/"]
COPY ["BooksBase.Shared/BooksBase.Shared.csproj", "BooksBase.Shared/"]
RUN dotnet restore "Web/Web.csproj"
COPY . .
WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]