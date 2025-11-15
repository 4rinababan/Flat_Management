#!/bin/bash
set -e

echo "⏳ Waiting for MySQL to be ready..."

until mysqladmin ping -h"$DB_HOST" --silent; do
  echo "MySQL not ready, retrying..."
  sleep 2
done

echo "MySQL is ready! Running EF Core migrations..."

dotnet MyApp.Web.dll
