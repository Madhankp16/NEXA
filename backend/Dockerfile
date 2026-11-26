# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["NexaWork.Domain/NexaWork.Domain.csproj", "NexaWork.Domain/"]
COPY ["NexaWork.Application/NexaWork.Application.csproj", "NexaWork.Application/"]
COPY ["NexaWork.Infrastructure/NexaWork.Infrastructure.csproj", "NexaWork.Infrastructure/"]
COPY ["NexaWork.Api/NexaWork.Api.csproj", "NexaWork.Api/"]
COPY ["NexaWork.slnx", "./"]

RUN dotnet restore "NexaWork.Api/NexaWork.Api.csproj"

COPY . .
WORKDIR "/src/NexaWork.Api"
RUN dotnet build "NexaWork.Api.csproj" -c Release -o /app/build
RUN dotnet publish "NexaWork.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NexaWork.Api.dll"]
