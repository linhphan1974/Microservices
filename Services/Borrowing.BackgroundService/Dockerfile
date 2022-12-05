#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Borrowing.BackgroundService/Borrowing.BackgroundTask.csproj", "Services/Borrowing.BackgroundService/"]
COPY ["EventBus/RabbitMQEventBus/RabbitMQEventBus.csproj", "EventBus/RabbitMQEventBus/"]
RUN dotnet restore "Services/Borrowing.BackgroundService/Borrowing.BackgroundTask.csproj"
COPY . .
WORKDIR "/src/Services/Borrowing.BackgroundService"
RUN dotnet build "Borrowing.BackgroundTask.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Borrowing.BackgroundTask.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Borrowing.BackgroundTask.dll"]