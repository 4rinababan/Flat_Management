# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["src/MyApp.Web/MyApp.Web.csproj", "MyApp.Web/"]
COPY ["src/MyApp.Core/MyApp.Core.csproj", "MyApp.Core/"]
COPY ["src/MyApp.Infrastructure/MyApp.Infrastructure.csproj", "MyApp.Infrastructure/"]
RUN dotnet restore "MyApp.Web/MyApp.Web.csproj"

# Copy everything else
COPY . .
WORKDIR "/src/MyApp.Web"

# Publish (fix untuk Blazor Server / minimal hosting)
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 80

# Start app
ENTRYPOINT ["dotnet", "MyApp.Web.dll"]
