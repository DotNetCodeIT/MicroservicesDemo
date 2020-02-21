#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-buster-slim-arm32v7 AS base #USE For Raspebbry PI
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Services/Customers/Customers.csproj", "Services/Customers/"]
RUN dotnet restore "Services/Customers/Customers.csproj"
COPY . .
WORKDIR "/src/Services/Customers"
RUN dotnet build "Customers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Customers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customers.dll"]