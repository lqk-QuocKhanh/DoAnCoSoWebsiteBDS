using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Infrastructure.Identity;

namespace VietPropEstate.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    // Vietnam address reference data
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<Ward> Wards => Set<Ward>();

    // Property domain
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyType> PropertyTypes => Set<PropertyType>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<TransactionType> TransactionTypes => Set<TransactionType>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<PropertyView> PropertyViews => Set<PropertyView>();

    // People domain
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    // Messaging domain
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<UserConversationSetting> UserConversationSettings => Set<UserConversationSetting>();
    public DbSet<UserBlock> UserBlocks => Set<UserBlock>();

    // Subscription & payment domain
    public DbSet<VIPPackage> VIPPackages => Set<VIPPackage>();
    public DbSet<UserVIPPackage> UserVIPPackages => Set<UserVIPPackage>();
    public DbSet<Payment> Payments => Set<Payment>();

    // Infrastructure domain
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync();
        return result;
    }

    private void ApplyAuditInformation()
    {
        var now = DateTime.UtcNow;
        var userId = _currentUserService.UserId;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = now;
                    entry.Entity.LastModifiedBy = userId;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = userId;
                    break;
            }
        }
    }

    private Task DispatchDomainEventsAsync()
    {
        var entities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entities)
            entity.ClearDomainEvents();

        return Task.CompletedTask;
    }
}
