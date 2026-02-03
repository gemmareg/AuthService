using AuthService.Domain.Abstractions;

namespace AuthService.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity, IAuditableEntity
    {
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public string CreatedBy { get; protected set; } = string.Empty;
        public string? UpdatedBy { get; protected set; }

        public void SetCreated(string createdBy)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
        }

        public void SetUpdated(string updatedBy)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}
