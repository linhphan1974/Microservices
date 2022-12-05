#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Clients/MvcClient/MvcClient.csproj", "Clients/MvcClient/"]
RUN dotnet restore "Clients/MvcClient/MvcClient.csproj"
COPY . .
WORKDIR "/src/Clients/MvcClient"
RUN dotnet build "MvcClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MvcClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MvcClient.dll"]