# Auth.Authorization

Autorización por permisos, declarativa, para APIs ASP.NET Core que validan JWT emitidos por **AuthService**. Añade un atributo `[RequiresPermission("...")]` y el filtro de MVC que lo hace efectivo, reutilizando los claims de permiso que ya lleva el token (vía [Auth.Contracts](https://www.nuget.org/packages/Auth.Contracts)).

No sustituye la autenticación: sigue haciendo falta que tu servicio valide el JWT (`AddJwtBearer(...)`) y que las acciones lleven `[Authorize]`. Este paquete solo añade la comprobación de "¿tiene este permiso concreto?" por encima de eso.

## Instalación

```bash
dotnet add package Auth.Authorization
```

## Configuración

En el arranque de tu API (`Program.cs` o donde registres tus servicios):

```csharp
using Auth.Authorization.Extensions;

builder.Services.AddAuthAuthorization();
```

Esto registra el filtro de permisos como filtro global de MVC. No hace falta llamarlo antes ni después de `AddControllers()` — solo añade configuración adicional a `MvcOptions`.

## Uso

```csharp
using Auth.Authorization.Attributes;
using Auth.Contracts;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    [RequiresPermission(AuthPermissions.RolesCreate)]
    [HttpPost]
    public async Task<ActionResult> Create(CreateRoleRequest request) { ... }

    [RequiresPermission(AuthPermissions.RolesRead)]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id) { ... }
}
```

`[RequiresPermission]` se puede aplicar a nivel de acción (como arriba, recomendado si distintas acciones necesitan distintos permisos) o a nivel de controller (si todas las acciones requieren el mismo permiso). Si el usuario no lo tiene, la petición se corta con `403 Forbidden` antes de llegar al cuerpo de la acción.

Los usuarios con el rol `Admin` pasan cualquier comprobación de permiso automáticamente (ver `Auth.Contracts.Extensions.UserExtensions.HasPermission`).

## Requisitos

- .NET 8, ASP.NET Core (usa `FrameworkReference` a `Microsoft.AspNetCore.App`, no hace falta ninguna otra dependencia de MVC).
- Un middleware de autenticación JWT ya configurado que popule `HttpContext.User` con los claims del token de AuthService.
