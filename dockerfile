# ======================
#  BUILD STAGE
# ======================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy hanya file csproj untuk caching restore
COPY src/MyApp.Web/*.csproj ./MyApp.Web/
COPY src/MyApp.Core/*.csproj ./MyApp.Core/
COPY src/MyApp.Infrastructure/*.csproj ./MyApp.Infrastructure/
COPY MyApp.sln .

# Restore dependencies
RUN dotnet restore MyApp.sln

# Copy seluruh source code setelah restore (agar cache tidak invalid tiap perubahan kecil)
COPY . .

# Build & publish project
RUN dotnet publish src/MyApp.Web/MyApp.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

# ======================
#  RUNTIME STAGE
# ======================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy hasil publish dari stage build
COPY --from=build /app/publish .

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Development

# Jalankan aplikasi
ENTRYPOINT ["dotnet", "MyApp.Web.dll"]
