# Comandos úteis — Backend, banco e Keycloak

Execute os comandos a partir da raiz do repositório, salvo quando indicado.

## Compilar a solução

```powershell
dotnet build api\Feak\Feak.SistemaFinanceiro.sln
```

Build de produção:

```powershell
dotnet build api\Feak\Feak.SistemaFinanceiro.sln -c Release
```

## Executar a API

```powershell
dotnet run --project api\Feak\Feak.SistemaFinanceiro.API\Feak.SistemaFinanceiro.API.csproj
```

Com recarregamento automático:

```powershell
dotnet watch --project api\Feak\Feak.SistemaFinanceiro.API\Feak.SistemaFinanceiro.API.csproj run
```

API: `http://localhost:7000`

Swagger: `http://localhost:7000/swagger`

## Executar testes

```powershell
dotnet test api\Feak\Feak.SistemaFinanceiro.sln
```

## Entity Framework — migrations

Entrar na pasta usada pelos comandos do EF:

```powershell
Set-Location api\Feak
```

Criar uma migration:

```powershell
dotnet ef migrations add NomeDaMigration --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API
```

Listar migrations e identificar as pendentes:

```powershell
dotnet ef migrations list --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API
```

Aplicar todas as migrations pendentes:

```powershell
dotnet ef database update --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API
```

Voltar o banco para uma migration específica:

```powershell
dotnet ef database update NomeDaMigrationAnterior --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API
```

Remover a última migration ainda não aplicada:

```powershell
dotnet ef migrations remove --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API
```

> Não use `migrations remove --force` em uma migration aplicada. Isso pode reverter o schema e remover dados.

Gerar o script SQL de todas as migrations de forma idempotente:

```powershell
dotnet ef migrations script --idempotent --project Feak.SistemaFinanceiro.Persistence --startup-project Feak.SistemaFinanceiro.API --output migrations.sql
```

Voltar para a raiz do repositório:

```powershell
Set-Location ..\..
```

## PostgreSQL da API

Iniciar:

```powershell
Set-Location api\Feak\Feak.SistemaFinanceiro.Application\services\docker
docker compose -p db-feak up -d
Set-Location ..\..\..\..\..
```

Ver logs:

```powershell
docker logs postgres-api --tail 100
```

Acessar o `psql`:

```powershell
docker exec -it postgres-api psql -U root -d db_postgres
```

Comandos úteis dentro do `psql`:

```sql
\dt feak_sf.*
SELECT id, nome_login, dh_inclusao, dh_exclusao FROM feak_sf.usuarios;
\q
```

Parar o banco sem apagar os dados:

```powershell
Set-Location api\Feak\Feak.SistemaFinanceiro.Application\services\docker
docker compose -p db-feak down
Set-Location ..\..\..\..\..
```

> `docker compose down -v` remove os volumes e apaga o banco. Use somente quando a exclusão dos dados for intencional.

## Keycloak e SPI

Iniciar sem reconstruir o SPI:

```powershell
Set-Location spi-keycloak
docker compose -p spi-feak up -d
Set-Location ..
```

Reconstruir o SPI e reiniciar o Keycloak:

```powershell
Set-Location spi-keycloak
docker compose -p spi-feak up -d --build
Set-Location ..
```

Ver logs recentes:

```powershell
docker logs feak-keycloak --tail 100
```

Acompanhar logs em tempo real:

```powershell
docker logs feak-keycloak --tail 100 -f
```

Reiniciar somente o Keycloak:

```powershell
docker restart feak-keycloak
```

Parar Keycloak e seu banco sem remover os volumes:

```powershell
Set-Location spi-keycloak
docker compose -p spi-feak down
Set-Location ..
```

## Verificar containers

```powershell
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
```

Containers esperados:

| Container | Porta |
| --- | --- |
| `postgres-api` | `5433` |
| `feak-keycloak-postgres` | `5432` |
| `feak-keycloak` | `8080` |

## Configuração JDBC do SPI

Para desenvolvimento com Docker Desktop:

```text
jdbc:postgresql://host.docker.internal:5433/db_postgres?currentSchema=feak_sf
```

Depois de alterar o código Java do SPI, execute novamente o build do container do Keycloak.

