#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-buster-slim-arm32v7 AS base #USE For Raspebbry PI
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ShopWebSite/ShopWebSite.csproj", "ShopWebSite/"]
RUN dotnet restore "ShopWebSite/ShopWebSite.csproj"
COPY . .
WORKDIR "/src/ShopWebSite"
RUN dotnet build "ShopWebSite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShopWebSite.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShopWebSite.dll"]