namespace AuthService.Domain.Abstractions
{
    public interface IAuditableEntity : IEntity
    {
        public DateTime CreatedAt { get;}
        public DateTime UpdatedAt { get; }
        public string CreatedBy { get; }
        public string UpdatedBy { get; }
    }
}
