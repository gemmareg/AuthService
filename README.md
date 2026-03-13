# AuthService

## Ejecutar con Docker (API + SQL Server + RabbitMQ)

La solución queda dockerizada con todas sus dependencias de infraestructura:

- `sqlserver`: Base de datos SQL Server 2022
- `migrator`: contenedor EF Core que automatiza `add-migration initial-migration` (solo si no existe ninguna) y `update-database`
- `rabbitmq`: Broker RabbitMQ con panel de administración
- `authservice`: API ASP.NET Core

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
- RabbitMQ UI: `http://localhost:15672`
- SQL Server: `localhost,1433` (usuario `sa`)

## Notas

- El migrator automatiza el bootstrap EF para bases nuevas:
  - crea `initial-migration` solo si no hay migraciones en `AuthService.Infrastructure/Migrations`.
  - ejecuta siempre `database update`.
- La API también aplica migraciones al arrancar como red de seguridad.
- `JwtSettings` y demás configuración se sobreescriben vía variables de entorno (`__` en claves anidadas).
- Define `ADMIN_SEED_USER_ID` para garantizar que el administrador tenga el mismo `UserId` en todos los sistemas.
- Cuando el admin se crea por seeding, se publica el evento `admin.created` en el exchange `auth.events`.
