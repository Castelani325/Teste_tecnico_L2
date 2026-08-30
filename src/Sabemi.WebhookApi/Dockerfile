# Etapa 1: build (usa a imagem do SDK, que tem as ferramentas de compilação)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Sabemi.WebhookApi.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime (imagem enxuta, só com o necessário pra RODAR, não compilar)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Sabemi.WebhookApi.dll"]
