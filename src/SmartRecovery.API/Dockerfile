# ── Estágio 1: Build ──────────────────────────────────────────────────────────
# Usamos a imagem do SDK para compilar o projeto.
# Esse estágio não vai para a imagem final — serve apenas para a compilação.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia apenas os arquivos .csproj primeiro.
# Isso aproveita o cache do Docker: se os pacotes não mudaram,
# o `dotnet restore` não é executado novamente.
COPY SmartRecovery.sln .
COPY src/SmartRecovery.API/SmartRecovery.API.csproj                       src/SmartRecovery.API/
COPY src/SmartRecovery.Application/SmartRecovery.Application.csproj       src/SmartRecovery.Application/
COPY src/SmartRecovery.Domain/SmartRecovery.Domain.csproj                 src/SmartRecovery.Domain/
COPY src/SmartRecovery.Infrastructure/SmartRecovery.Infrastructure.csproj src/SmartRecovery.Infrastructure/

RUN dotnet restore src/SmartRecovery.API/SmartRecovery.API.csproj

# Copia o restante do código-fonte e publica o projeto da API
COPY . .
RUN dotnet publish src/SmartRecovery.API/SmartRecovery.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Estágio 2: Runtime ────────────────────────────────────────────────────────
# A imagem final usa apenas o runtime (muito menor que o SDK).
# Resultado: imagem final leve e segura.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Desativa W^X para compatibilidade com o kernel/seccomp do Render (evita Exit Status 139 / SIGSEGV)
ENV DOTNET_EnableWriteXorExecute=0
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "SmartRecovery.API.dll"]
