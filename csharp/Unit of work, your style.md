# Unit of work my way

- Unit of work => to ensure we commit data to db only when other operations also succeed
- If we are injecting an EF Core `dbContext` already, we can use it's `transactions` feature; but if we use a repository pattern, then this is not possible/not so clean. 
- Thus, we create a layer on top of repository called the Unit of Work, where in this layer it is where we control the persistence to the db, and even add custom uniform behavior before commiting.

- Example:
```
public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
```

```
// Generic Repository
public interface IRepository<T> where T : class
{
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T> GetByIdAsync(int id);
}

// The Unit Of Work class here defines the concrete objects whose committing behavior is to be controlled by unit
public interface IUnitOfWork : IDisposable
{
    IRepository<Customer> Customers { get; }
    IRepository<Order> Orders { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}
```

```
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public void Update(T entity) => _dbSet.Update(entity);
    public void Delete(T entity) => _dbSet.Remove(entity);
    public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
}


public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;

    public IRepository<Customer> Customers { get; }
    public IRepository<Order> Orders { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Customers = new Repository<Customer>(_context);
        Orders = new Repository<Order>(_context);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
```

```
public async Task PlaceOrderAsync(Order newOrder)
{
    using var unitOfWork = new UnitOfWork(_context);

    await unitOfWork.BeginTransactionAsync();

    try
    {
        // Stage data to be committed to db later st the final stage
        await unitOfWork.Orders.AddAsync(newOrder);
        // Add logic like notifying an api, publishing event to bus

        await unitOfWork.CommitAsync(); // Saves + commits
    }
    catch
    {
        // If the logic above throws, the data won't be committed, ensuring integrity!!!
        await unitOfWork.RollbackAsync();
        throw;
    }
}
```