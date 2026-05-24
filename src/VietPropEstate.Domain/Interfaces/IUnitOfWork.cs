using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPropertyRepository Properties { get; }
    IRepository<Agent> Agents { get; }
    IRepository<Customer> Customers { get; }
    IRepository<PropertyType> PropertyTypes { get; }
    IRepository<TransactionType> TransactionTypes { get; }
    IRepository<Transaction> Transactions { get; }
    IRepository<Favorite> Favorites { get; }
    IRepository<PropertyView> PropertyViews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
