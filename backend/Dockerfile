FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем файл проекта (обратите внимание - TodoApi.csproj)
COPY *.csproj .
RUN dotnet restore

# Копируем все остальные файлы
COPY . .

# Публикуем приложение
RUN dotnet publish -c Release -o out

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80

# Копируем опубликованные файлы
COPY --from=build /app/out .

# Запускаем с правильным именем DLL
ENTRYPOINT ["dotnet", "TodoApi.dll"]