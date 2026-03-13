# AuthService

## Ejecutar con Docker (API + SQL Server + RabbitMQ)

La solución queda dockerizada con todas sus dependencias de infraestructura:

- `authservice`: API ASP.NET Core
- `sqlserver`: Base de datos SQL Server 2022
- `rabbitmq`: Broker RabbitMQ con panel de administración

### 1) Levantar contenedores

```bash
docker compose up --build -d
```

### 2) Ver logs de la API

```bash
docker compose logs -f authservice
```

### 3) Endpoints y puertos

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- RabbitMQ UI: `http://localhost:15672` (usuario `guest`, contraseña `guest`)
- SQL Server: `localhost,1433` (usuario `sa`)

### Notas

- Las migraciones de Entity Framework se aplican automáticamente al iniciar la API.
- Se ejecuta el seeding inicial, incluyendo roles base y usuario administrador.
- Usuario administrador inicial configurable mediante `AdminSeed` en `appsettings.json`/variables de entorno.
- Las credenciales y secretos definidos en `docker-compose.yml` son para entorno local/desarrollo.
