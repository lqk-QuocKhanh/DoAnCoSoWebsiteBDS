using Microsoft.EntityFrameworkCore;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Vietnam address reference data
    DbSet<Province> Provinces { get; }
    DbSet<Ward> Wards { get; }

    // Property domain
    DbSet<Property> Properties { get; }
    DbSet<PropertyType> PropertyTypes { get; }
    DbSet<PropertyImage> PropertyImages { get; }
    DbSet<TransactionType> TransactionTypes { get; }
    DbSet<Favorite> Favorites { get; }
    DbSet<PropertyView> PropertyViews { get; }

    // People domain
    DbSet<Agent> Agents { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Transaction> Transactions { get; }

    // Messaging domain
    DbSet<Conversation> Conversations { get; }
    DbSet<Message> Messages { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<UserConversationSetting> UserConversationSettings { get; }
    DbSet<UserBlock> UserBlocks { get; }

    // Subscription & payment domain
    DbSet<VIPPackage> VIPPackages { get; }
    DbSet<UserVIPPackage> UserVIPPackages { get; }
    DbSet<Payment> Payments { get; }

    // Infrastructure domain
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
