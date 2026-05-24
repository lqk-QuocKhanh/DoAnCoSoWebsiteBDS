using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

/// <summary>Represents one user blocking another from messaging.</summary>
public class UserBlock : AuditableEntity
{
    public string BlockerId { get; private set; } = string.Empty;
    public string BlockedUserId { get; private set; } = string.Empty;

    private UserBlock() { }

    public static UserBlock Create(string blockerId, string blockedUserId)
    {
        if (string.IsNullOrWhiteSpace(blockerId))
            throw new DomainException("Blocker ID is required.");
        if (string.IsNullOrWhiteSpace(blockedUserId))
            throw new DomainException("Blocked user ID is required.");
        if (blockerId == blockedUserId)
            throw new DomainException("You cannot block yourself.");

        return new UserBlock
        {
            BlockerId = blockerId,
            BlockedUserId = blockedUserId
        };
    }
}
