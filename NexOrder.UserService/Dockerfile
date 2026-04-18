# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NexOrder.UserService/NexOrder.UserService.csproj", "NexOrder.UserService/"]
COPY ["NexOrder.UserService.Application/NexOrder.UserService.Application.csproj", "NexOrder.UserService.Application/"]
COPY ["NexOrder.UserService.Domain/NexOrder.UserService.Domain.csproj", "NexOrder.UserService.Domain/"]
COPY ["NexOrder.UserService.Messages/NexOrder.UserService.Messages.csproj", "NexOrder.UserService.Messages/"]
COPY ["NexOrder.UserService.Shared/NexOrder.UserService.Shared.csproj", "NexOrder.UserService.Shared/"]
COPY ["NexOrder.UserService.Infrastructure/NexOrder.UserService.Infrastructure.csproj", "NexOrder.UserService.Infrastructure/"]

ARG GITHUB_USERNAME
ARG GITHUB_TOKEN

RUN dotnet nuget add source https://nuget.pkg.github.com/$GITHUB_USERNAME/index.json \
    --name github \
    --username $GITHUB_USERNAME \
    --password $GITHUB_TOKEN \
    --store-password-in-clear-text
RUN dotnet restore "./NexOrder.UserService/NexOrder.UserService.csproj"
COPY . .
WORKDIR "/src/NexOrder.UserService"
RUN dotnet build "./NexOrder.UserService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NexOrder.UserService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true