#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim-arm32v7 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["FiskalApp.csproj", "./"]
COPY ["foo/Fiskal.Model/Fiskal.Model.csproj", "foo/Fiskal.Model/"]
COPY ["foo/Fiskal/Fiskal.csproj", "foo/Fiskal/"]
RUN dotnet restore "FiskalApp.csproj" -r linux-arm
COPY . .
WORKDIR "/src/"
RUN dotnet build "FiskalApp.csproj" -c Release -o /app/build -r linux-arm

FROM build AS publish
RUN dotnet publish "FiskalApp.csproj" -c Release -o /app/publish -r linux-arm

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FiskalApp.dll"]
