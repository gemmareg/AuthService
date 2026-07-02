namespace Auth.Contracts
{
    /// <summary>
    /// Nombres de claim tipo usados por AuthService al emitir tokens.
    /// Cualquier servicio que valide tokens emitidos por AuthService debe usar
    /// estas constantes (en vez de strings sueltos) para leer los claims,
    /// garantizando que el emisor (TokenService) y los consumidores nunca diverjan.
    /// </summary>
    public static class AuthClaimTypes
    {
        /// <summary>
        /// Claim tipo bajo el que se emite cada permiso efectivo del usuario
        /// (permisos directos + permisos heredados de sus roles, deduplicados).
        /// </summary>
        public const string Permission = "permission";
    }
}
