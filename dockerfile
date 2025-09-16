# -------------------------------
# Build stage
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj dan restore
COPY ["src/MyApp.Web/MyApp.Web.csproj", "MyApp.Web/"]
COPY ["src/MyApp.Core/MyApp.Core.csproj", "MyApp.Core/"]
COPY ["src/MyApp.Infrastructure/MyApp.Infrastructure.csproj", "MyApp.Infrastructure/"]
RUN dotnet restore "MyApp.Web/MyApp.Web.csproj"

# Copy semua source code
COPY . .
WORKDIR "/src/MyApp.Web"

# Publish untuk Linux runtime (Blazor Server minimal hosting fix)
RUN dotnet publish -c Release -o /app/publish -r linux-x64 --self-contained false /p:UseAppHost=false

# -------------------------------
# Runtime stage
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Start Blazor Server
ENTRYPOINT ["dotnet", "MyApp.Web.dll"]
