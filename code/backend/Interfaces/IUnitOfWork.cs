using System;

namespace backend.Interfaces;

public interface IUnitOfWork : IDisposable
{
    UserManager<ApplicationUser> Users { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}