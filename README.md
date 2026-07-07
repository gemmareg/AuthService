# AuthService

## Ejecutar con Docker (API + SQL Server)

La solución queda dockerizada con todas sus dependencias de infraestructura:

- `sqlserver`: Base de datos SQL Server 2022
- `migrator`: contenedor EF Core que automatiza `add-migration initial-migration` (solo si no existe ninguna) y `update-database`
- `authservice`: API ASP.NET Core

AuthService publica sus eventos de dominio (`user.registered`, `admin.created`, etc.) vía HTTP contra **MessageBrokerService**, usando el paquete `MessageBroker.Client` y un token propio de la cuenta de servicio `EventPublisherSeed`. MessageBrokerService no forma parte de este `docker-compose.yml`; debe estar levantado por separado y su URL se configura en `MessageBroker:BaseUrl`.

## 1) Preparar variables de entorno

```bash
cp .env.example .env
```

Ajusta los valores sensibles en `.env` antes de levantar el stack.

## 2) Levantar contenedores

```bash
docker compose up --build -d
```

## 3) Ver logs

```bash
docker compose logs -f migrator
docker compose logs -f authservice
```

## 4) Endpoints y puertos

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- SQL Server: `localhost,1433` (usuario `sa`)

## Ejecutar sin Docker (desarrollo local)

```bash
cp AuthService.Host/appsettings.Development.json.example AuthService.Host/appsettings.Development.json
```

Rellena los valores marcados como `CHANGE_ME` (cadena de conexión, `JwtSettings:SecretKey`, password del admin seed, password del `EventPublisherSeed`) antes de arrancar `AuthService.Host`. Este archivo está en `.gitignore`: nunca se commitea con valores reales.

## Notas

- El migrator automatiza el bootstrap EF para bases nuevas:
  - crea `initial-migration` solo si no hay migraciones en `AuthService.Infrastructure/Migrations`.
  - ejecuta siempre `database update`.
- La API también aplica migraciones al arrancar como red de seguridad.
- `JwtSettings` y demás configuración se sobreescriben vía variables de entorno (`__` en claves anidadas).
- Define `ADMIN_SEED_USER_ID` para garantizar que el administrador tenga el mismo `UserId` en todos los sistemas.
- El seeder también crea, antes que el Admin, la cuenta de servicio `EventPublisherSeed` (rol `EventPublisher`, permiso `events:publish`) que AuthService usa para autenticarse a sí mismo contra MessageBrokerService.
- Cuando el admin se crea por seeding, se publica el evento `admin.created` contra MessageBrokerService.
