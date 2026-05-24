using Microsoft.EntityFrameworkCore.Storage;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Interfaces;
using VietPropEstate.Infrastructure.Persistence.Repositories;

namespace VietPropEstate.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    private IRepository<Agent>? _agents;
    private IRepository<Customer>? _customers;
    private IRepository<PropertyType>? _propertyTypes;
    private IRepository<TransactionType>? _transactionTypes;
    private IRepository<Transaction>? _transactions;
    private IRepository<Favorite>? _favorites;
    private IRepository<PropertyView>? _propertyViews;

    public UnitOfWork(ApplicationDbContext context, IPropertyRepository propertyRepository)
    {
        _context = context;
        Properties = propertyRepository;
    }

    public IPropertyRepository Properties { get; }

    public IRepository<Agent> Agents =>
        _agents ??= new GenericRepository<Agent>(_context);

    public IRepository<Customer> Customers =>
        _customers ??= new GenericRepository<Customer>(_context);

    public IRepository<PropertyType> PropertyTypes =>
        _propertyTypes ??= new GenericRepository<PropertyType>(_context);

    public IRepository<TransactionType> TransactionTypes =>
        _transactionTypes ??= new GenericRepository<TransactionType>(_context);

    public IRepository<Transaction> Transactions =>
        _transactions ??= new GenericRepository<Transaction>(_context);

    public IRepository<Favorite> Favorites =>
        _favorites ??= new GenericRepository<Favorite>(_context);

    public IRepository<PropertyView> PropertyViews =>
        _propertyViews ??= new GenericRepository<PropertyView>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("No active transaction.");
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("No active transaction.");
        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }
}
