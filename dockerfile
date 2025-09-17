# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# jadikan root solution sebagai workdir
WORKDIR /app

# copy semua file ke /app (tidak membuat src dobel)
COPY . .

# restore solution
RUN dotnet restore MyApp.sln

# publish project web
RUN dotnet publish src/MyApp.Web/MyApp.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=development
ENTRYPOINT ["dotnet", "MyApp.Web.dll"]
