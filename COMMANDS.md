# FEAK - Comandos Úteis

## Pré-requisitos
- Docker Desktop (em execução)
- .NET 8 SDK
- Node.js 18+ e npm
- Angular CLI (`npm install -g @angular/cli`)

---

## 1. Banco de dados da API (PostgreSQL na porta 5433)

```powershell
# Iniciar
Set-Location "api\Feak\Feak.SistemaFinanceiro.Application\services\docker"
docker compose -p db-feak up -d

# Parar
docker compose -p db-feak down

# Parar e remover volume (limpa o banco inteiro)
docker compose -p db-feak down -v
```

---

## 2. Keycloak + banco Keycloak (porta 8080)

```powershell
# Iniciar (sem rebuild)
Set-Location "spi-keycloak"
docker compose -p spi-feak up -d

# Iniciar com rebuild do SPI (após alterar o provider Java)
docker compose -p spi-feak up -d --build

# Parar
docker compose -p spi-feak down

# Parar e remover volumes (reseta Keycloak do zero)
docker compose -p spi-feak down -v
```

**Admin Keycloak:** http://localhost:8080  
**Usuário:** `admin` | **Senha:** `admin`

---

## 3. API .NET (porta padrão 5000 / 5001)

```powershell
Set-Location "api\Feak\Feak.SistemaFinanceiro.API"

# Rodar em desenvolvimento
dotnet run

# Ou com watch (reinicia ao salvar)
dotnet watch run
```

> A connection string usa `Port=5433` — certifique-se de que o container `postgres-api` está rodando antes.

---

## 4. Migrations do Entity Framework

```powershell
Set-Location "api\Feak"

# Aplicar todas as migrations pendentes
dotnet ef database update --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API

# Criar nova migration
dotnet ef migrations add <NomeDaMigration> --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API

# Reverter para uma migration específica
dotnet ef database update <NomeDaMigration> --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API

# Limpar schema e reaplicar tudo do zero (via psql no container)
docker exec -it postgres-api psql -U root -d db_postgres -c "DROP SCHEMA IF EXISTS feak_sf CASCADE; CREATE SCHEMA feak_sf;"
dotnet ef database update --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API
```

---

## 5. Frontend Angular (porta 4200)

```powershell
Set-Location "web"

# Instalar dependências (primeira vez ou após atualizar package.json)
npm install

# Rodar em desenvolvimento
ng serve
# ou
npm start

# Build de produção
ng build --configuration production
```

**App:** http://localhost:4200

---

## 6. Subir tudo de uma vez

```powershell
# 1. Banco da API
Set-Location "api\Feak\Feak.SistemaFinanceiro.Application\services\docker"; docker compose -p db-feak up -d; Set-Location "..\..\..\..\..\"

# 2. Keycloak
Set-Location "spi-keycloak"; docker compose -p spi-feak up -d; Set-Location ".."

# 3. Migrations
Set-Location "api\Feak"; dotnet ef database update --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API; Set-Location "..\.."

# 4. API (em outro terminal)
Set-Location "api\Feak\Feak.SistemaFinanceiro.API"; dotnet run

# 5. Frontend (em outro terminal)
Set-Location "web"; ng serve
```

---

## 7. Verificar status dos containers

```powershell
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
```

Containers esperados:

| Nome                    | Porta |
|-------------------------|-------|
| `postgres-api`          | 5433  |
| `feak-keycloak-postgres`| 5432  |
| `feak-keycloak`         | 8080  |

---

## 8. Logs dos containers

```powershell
docker logs postgres-api --tail 50
docker logs feak-keycloak --tail 100 -f
docker logs feak-keycloak-postgres --tail 50
```

---

## 9. Acesso direto ao banco via psql

```powershell
# Banco da API
docker exec -it postgres-api psql -U root -d db_postgres

# Listar tabelas do schema feak_sf
docker exec -it postgres-api psql -U root -d db_postgres -c "\dt feak_sf.*"

# Banco do Keycloak
docker exec -it feak-keycloak-postgres psql -U keycloak -d dbkeycloak
```
