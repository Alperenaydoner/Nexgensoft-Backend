# Render / Docker Hub: build context = repository root (Render looks for ./Dockerfile here).
# CoreService-only context: use services/core-service/CoreService/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY services/core-service/CoreService/CoreService.csproj services/core-service/CoreService/
RUN dotnet restore services/core-service/CoreService/CoreService.csproj

COPY services/core-service/CoreService/ services/core-service/CoreService/
WORKDIR /src/services/core-service/CoreService
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
CMD ["sh", "-c", "exec dotnet CoreService.dll --urls \"http://0.0.0.0:${PORT:-8080}\""]
