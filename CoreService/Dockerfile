# CoreService — local / CI when build context is this folder only.
# Render (repo root context): use Dockerfile at repository root.

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
