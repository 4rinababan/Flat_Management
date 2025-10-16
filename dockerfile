# ======================
#  BUILD STAGE
# ======================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Pastikan environment NuGet bersih
ENV NUGET_PACKAGES=/root/.nuget/fallbackpackages

# Copy csproj dan restore
COPY src/MyApp.Web/*.csproj src/MyApp.Web/
COPY src/MyApp.Core/*.csproj src/MyApp.Core/
COPY src/MyApp.Infrastructure/*.csproj src/MyApp.Infrastructure/
COPY MyApp.sln .

# Bersihkan cache restore lama
RUN dotnet nuget locals all --clear

# Restore dependensi (ulang dari 0, path Linux)
RUN dotnet restore MyApp.sln

# Copy seluruh source code
COPY . .

# Build & publish
RUN dotnet publish src/MyApp.Web/MyApp.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

# ======================
#  RUNTIME STAGE
# ======================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "MyApp.Web.dll"]
