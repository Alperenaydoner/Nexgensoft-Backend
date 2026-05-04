# Nexgensoft-Backend: build context = repository root where CoreService.csproj lives
# (no services/core-service/... path — that layout is for the full Nexgensoft monorepo only).
# Monorepo local build: docker build -f services/core-service/CoreService/Dockerfile services/core-service/CoreService

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY CoreService.csproj .
RUN dotnet restore CoreService.csproj
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
CMD ["sh", "-c", "exec dotnet CoreService.dll --urls \"http://0.0.0.0:${PORT:-8080}\""]
