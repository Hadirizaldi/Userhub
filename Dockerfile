# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj dulu lalu restore → layer ini ke-cache selama dependency nggak berubah.
COPY src/UserHub.Domain/UserHub.Domain.csproj                 src/UserHub.Domain/
COPY src/UserHub.Application/UserHub.Application.csproj        src/UserHub.Application/
COPY src/UserHub.Infrastructure/UserHub.Infrastructure.csproj src/UserHub.Infrastructure/
COPY src/UserHub.Web/UserHub.Web.csproj                       src/UserHub.Web/
RUN dotnet restore src/UserHub.Web/UserHub.Web.csproj

# Baru copy seluruh source + publish.
COPY src/ src/
RUN dotnet publish src/UserHub.Web/UserHub.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "UserHub.Web.dll"]