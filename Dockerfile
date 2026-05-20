FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 7000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["CodeInterviewPro.API/CodeInterviewPro.API.csproj", "CodeInterviewPro.API/"]
COPY ["CodeInterviewPro.Application/CodeInterviewPro.Application.csproj", "CodeInterviewPro.Application/"]
COPY ["CodeInterviewPro.Domain/CodeInterviewPro.Domain.csproj", "CodeInterviewPro.Domain/"]
COPY ["CodeInterviewPro.Infrastructure/CodeInterviewPro.Infrastructure.csproj", "CodeInterviewPro.Infrastructure/"]
RUN dotnet restore "./CodeInterviewPro.API/CodeInterviewPro.API.csproj"

# Copy the rest of the code and build
COPY . .
WORKDIR "/src/CodeInterviewPro.API"
RUN dotnet build "./CodeInterviewPro.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CodeInterviewPro.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose on port 7000 as configured in the app
ENV ASPNETCORE_URLS=http://+:7000

# Install Docker CLI so the container can execute docker commands (Docker-out-of-Docker)
USER root
RUN apt-get update && apt-get install -y docker.io && rm -rf /var/lib/apt/lists/*
USER app

ENTRYPOINT ["dotnet", "CodeInterviewPro.API.dll"]
