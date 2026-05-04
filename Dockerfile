# Yalnızca build context = repo kökü (Render "Root Directory" BOŞ) iken.
# GitHub yapın Nexgensoft-Backend/CoreService/ gibi ise Render'da Root Directory=CoreService
# kullanın ve bu dosyayı DEĞİL, CoreService/Dockerfile kullanın (context klasör içi olmalı).
#
# Bu dosya için (kök context): Render → Docker Build Args gerekirse SERVICE_DIR ayarlayın:
#   CoreService/ altında .csproj → SERVICE_DIR=CoreService (varsayılan)
#   .csproj repo kökünde          → SERVICE_DIR=.
#   Monorepo                       → SERVICE_DIR=services/core-service/CoreService

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG SERVICE_DIR=CoreService
WORKDIR /src
COPY ${SERVICE_DIR}/CoreService.csproj .
COPY ${SERVICE_DIR}/ .
RUN dotnet restore CoreService.csproj
RUN dotnet publish CoreService.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
CMD ["sh", "-c", "exec dotnet CoreService.dll --urls \"http://0.0.0.0:${PORT:-8080}\""]
