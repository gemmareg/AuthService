namespace Auth.Contracts
{
    /// <summary>
    /// Nombres de permisos "bien conocidos" que varios endpoints de AuthService
    /// (y potencialmente otros servicios que confíen en sus tokens) necesitan
    /// referenciar. Centralizarlos aquí evita magic strings duplicados y typos
    /// entre el controller, el seeder y cualquier consumidor externo.
    ///
    /// Convención uniforme para todo permiso: "recurso:accion" donde accion es
    /// siempre una de {read, create, update, delete}, y opcionalmente ":any"
    /// cuando hace falta distinguir una acción sobre cualquier recurso de la
    /// acción equivalente sobre el propio recurso del usuario autenticado (que
    /// normalmente no requiere permiso). Un recurso con solo lectura/escritura
    /// se documenta simplemente listando qué subconjunto de las cuatro
    /// acciones define.
    /// </summary>
    public static class AuthPermissions
    {
        /// <summary>Permite desactivar (soft-delete) la cuenta de otro usuario.</summary>
        public const string UsersDeleteAny = "users:delete:any";

        /// <summary>Permite editar los datos de otro usuario.</summary>
        public const string UsersUpdateAny = "users:update:any";

        /// <summary>Permite consultar roles (detalle o listado).</summary>
        public const string RolesRead = "roles:read";

        /// <summary>Permite crear roles.</summary>
        public const string RolesCreate = "roles:create";

        /// <summary>Permite renombrar un rol y asignarle/quitarle permisos.</summary>
        public const string RolesUpdate = "roles:update";

        /// <summary>Permite eliminar roles.</summary>
        public const string RolesDelete = "roles:delete";

        /// <summary>Permite consultar permisos (detalle o listado).</summary>
        public const string PermissionsRead = "permissions:read";

        /// <summary>Permite crear permisos.</summary>
        public const string PermissionsCreate = "permissions:create";

        /// <summary>Permite editar la descripción de un permiso y (des)activarlo.</summary>
        public const string PermissionsUpdate = "permissions:update";

        /// <summary>Permite eliminar permisos.</summary>
        public const string PermissionsDelete = "permissions:delete";

        // Los siguientes permisos son de recursos propiedad de MessageBrokerService, no de
        // AuthService. Se centralizan aquí (en vez de en el propio MessageBrokerService) porque
        // AuthService es quien emite y gestiona todos los permisos del ecosistema — conceder
        // estos permisos a un rol/usuario es responsabilidad suya, así que también lo es evitar
        // que su nombre viva como dos magic strings independientes que puedan divergir.

        /// <summary>Permite publicar eventos en MessageBrokerService.</summary>
        public const string EventsPublish = "events:publish";

        /// <summary>Permite consultar eventos publicados en MessageBrokerService (detalle o sus entregas).</summary>
        public const string EventsRead = "events:read";

        /// <summary>Permite crear suscripciones en MessageBrokerService.</summary>
        public const string SubscriptionsCreate = "subscriptions:create";

        /// <summary>Permite consultar suscripciones en MessageBrokerService.</summary>
        public const string SubscriptionsRead = "subscriptions:read";

        /// <summary>Permite activar/desactivar una suscripción en MessageBrokerService.</summary>
        public const string SubscriptionsUpdate = "subscriptions:update";

        /// <summary>Permite consultar entregas fallidas (dead-lettered) en MessageBrokerService.</summary>
        public const string DeliveriesRead = "deliveries:read";

        /// <summary>Permite reencolar una entrega fallida en MessageBrokerService.</summary>
        public const string DeliveriesRequeue = "deliveries:requeue";
    }
}
