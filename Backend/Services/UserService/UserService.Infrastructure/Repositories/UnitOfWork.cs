using UserService.Application.Abstractions;

namespace UserService.Infrastructure.Repositories;

public class UnitOfWork(UserServiceContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
