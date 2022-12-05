#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Borrow.Api/Borrowing.Api.csproj", "Services/Borrow.Api/"]
COPY ["EventBus/RabbitMQEventBus/RabbitMQEventBus.csproj", "EventBus/RabbitMQEventBus/"]
COPY ["EventBus/EventLogService/EventLogService.csproj", "EventBus/EventLogService/"]
RUN dotnet restore "Services/Borrow.Api/Borrowing.Api.csproj"
COPY . .
WORKDIR "/src/Services/Borrow.Api"
RUN dotnet build "Borrowing.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Borrowing.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Borrowing.Api.dll"]