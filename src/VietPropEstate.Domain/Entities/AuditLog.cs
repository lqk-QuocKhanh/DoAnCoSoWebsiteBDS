using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Domain.Entities;

/// <summary>Immutable audit trail entry — never updated or deleted once written.</summary>
public class AuditLog : BaseEntity
{
    /// <summary>Identity user ID of the actor — null for system/background actions.</summary>
    public string? UserId { get; private set; }

    public AuditAction Action { get; private set; }
    public string EntityName { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }

    /// <summary>JSON snapshot of the entity before the change.</summary>
    public string? OldValues { get; private set; }

    /// <summary>JSON snapshot of the entity after the change.</summary>
    public string? NewValues { get; private set; }

    /// <summary>Changed property names, serialised as JSON array.</summary>
    public string? AffectedColumns { get; private set; }

    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime Timestamp { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        AuditAction action,
        string entityName,
        string? entityId = null,
        string? userId = null,
        string? oldValues = null,
        string? newValues = null,
        string? affectedColumns = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        return new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            UserId = userId,
            OldValues = oldValues,
            NewValues = newValues,
            AffectedColumns = affectedColumns,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Timestamp = DateTime.UtcNow
        };
    }
}
