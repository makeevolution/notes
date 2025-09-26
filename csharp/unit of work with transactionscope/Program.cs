using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

#region Models

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
}

#endregion

#region DbContext

public class AppDbContext : DbContext
{
    public DbSet<Person> People { get; set; }

    private readonly string _dbPath;

    public AppDbContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={_dbPath}");
    }
}

#endregion

#region Repository

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Remove(T entity);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _context.Set<T>().ToListAsync();

    public async Task AddAsync(T entity) =>
        await _context.Set<T>().AddAsync(entity);

    public void Remove(T entity) =>
        _context.Set<T>().Remove(entity);
}

#endregion

#region UnitOfWork

public interface IUnitOfWork : IDisposable
{
    IRepository<Person> People { get; }
    Task BeginAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;

    public IRepository<Person> People { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        People = new Repository<Person>(_context);
    }

    public async Task BeginAsync()
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

    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}

#endregion

#region Service

public class PersonService
{
    private readonly IUnitOfWork _uow;

    public PersonService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task AddPeopleWithRollbackAsync()
    {
        await _uow.BeginAsync();
        try
        {
            await _uow.People.AddAsync(new Person { Name = "Alice" });
            await _uow.People.AddAsync(new Person { Name = "Bob" });

            // Simulate failure
            // throw new Exception("Simulated failure");

            await _uow.CommitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            await _uow.RollbackAsync();
        }
    }

    public async Task ListPeopleAsync()
    {
        var people = await _uow.People.GetAllAsync();
        Console.WriteLine("People in DB:");
        foreach (var person in people)
        {
            Console.WriteLine($"- {person.Name}");
        }
    }
}

#endregion

#region Program

class Program
{
    static async Task Main()
    {
        var dbPath = Path.Combine(Environment.CurrentDirectory, "test.db");
        Console.WriteLine($"Using DB: {dbPath}");

        // Clean up and ensure fresh DB
        if (File.Exists(dbPath)) File.Delete(dbPath);

        using (var context = new AppDbContext(dbPath))
        {
            await context.Database.EnsureCreatedAsync();
        }

        using (var context = new AppDbContext(dbPath))
        using (var uow = new UnitOfWork(context))
        {
            var service = new PersonService(uow);
            await service.AddPeopleWithRollbackAsync();
        }

        // Confirm rollback
        using (var context = new AppDbContext(dbPath))
        using (var uow = new UnitOfWork(context))
        {
            var service = new PersonService(uow);
            await service.ListPeopleAsync(); // Should be empty
        }
    }
}

#endregion
