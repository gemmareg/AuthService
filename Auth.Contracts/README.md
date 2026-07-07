# Auth.Contracts

Contratos compartidos para servicios que consumen tokens JWT o eventos publicados por **AuthService**: constantes de tipos de claim/permiso, extensiones sobre `ClaimsPrincipal` para leer esos claims, y los records de los eventos de integración que AuthService publica en **MessageBrokerService**.

Sin dependencias de ASP.NET Core: se puede usar desde cualquier tipo de proyecto .NET (una API, un worker, una función serverless) siempre que ya tengas un `ClaimsPrincipal` autenticado a partir del JWT de AuthService.

## Instalación

```bash
dotnet add package Auth.Contracts
```

## Leer claims del usuario autenticado

```csharp
using Auth.Contracts.Extensions;

string userId = User.GetId();
string email = User.GetEmail();
List<string> roles = User.GetRoles();
bool isAdmin = User.IsAdmin();
bool canDeleteAny = User.HasPermission(AuthPermissions.UsersDeleteAny);
```

`HasPermission` siempre devuelve `true` para usuarios con el rol `Admin`, sin mirar la lista de permisos — AuthService ya se asegura de que el token de un Admin lleve todos los permisos activos del sistema, así que este atajo es solo eso, un atajo.

## Nombres de permisos conocidos

`AuthPermissions` centraliza los nombres de permiso que expone AuthService, para no repetir strings sueltos:

```csharp
AuthPermissions.UsersDeleteAny;      // "users:delete:any"
AuthPermissions.UsersUpdateAny;      // "users:update:any"
AuthPermissions.RolesRead;           // "roles:read"
AuthPermissions.RolesCreate;         // "roles:create"
AuthPermissions.RolesUpdate;         // "roles:update"
AuthPermissions.RolesDelete;         // "roles:delete"
AuthPermissions.PermissionsRead;     // "permissions:read"
AuthPermissions.PermissionsCreate;   // "permissions:create"
AuthPermissions.PermissionsUpdate;   // "permissions:update"
AuthPermissions.PermissionsDelete;   // "permissions:delete"
```

Convención: `recurso:accion`, con `accion` siempre en `{read, create, update, delete}`, y opcionalmente `:any` cuando hace falta distinguir una acción sobre cualquier recurso de la acción equivalente sobre el propio recurso del usuario autenticado.

### Permisos de otros servicios

`AuthPermissions` también centraliza el vocabulario de permisos de recursos que no son de AuthService pero cuyos tokens se emiten y gestionan aquí — hoy, los de **MessageBrokerService**:

```csharp
AuthPermissions.EventsPublish;        // "events:publish"
AuthPermissions.EventsRead;           // "events:read"
AuthPermissions.SubscriptionsCreate;  // "subscriptions:create"
AuthPermissions.SubscriptionsRead;    // "subscriptions:read"
AuthPermissions.SubscriptionsUpdate;  // "subscriptions:update"
AuthPermissions.DeliveriesRead;       // "deliveries:read"
AuthPermissions.DeliveriesRequeue;    // "deliveries:requeue"
```

## Eventos de integración

AuthService publica sus eventos de dominio contra **MessageBrokerService** (vía HTTP, con el paquete `MessageBroker.Client`). Si tu servicio se suscribe a alguno de estos `eventType` en MessageBrokerService, estos son los records a los que deserializar el `payload` de cada uno:

| eventType | Tipo |
|---|---|
| `user.registered` | `UserRegisteredEvent` |
| `user.soft-deleted` | `UserSoftDeletedEvent` |
| `user.activated` | `UserActivatedEvent` |
| `user.email-changed` | `EmailChangedEvent` |
| `admin.created` | `AdminCreatedEvent` |

Todos viven en el namespace `Auth.Contracts.Events`.

## Ver también

- [Auth.Authorization](https://www.nuget.org/packages/Auth.Authorization) — si además de leer claims necesitas proteger endpoints por permiso con un atributo declarativo.
