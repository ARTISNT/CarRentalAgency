using Microsoft.EntityFrameworkCore;
using UserService.Domain.Users;

namespace UserService.Infrastructure.Repositories;

public class UserRepository(UserServiceContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FindAsync(id, cancellationToken);
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);
        return user;
    }

    public async Task<IReadOnlyCollection<User>?> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await context.Users.ToListAsync(cancellationToken);
        return users;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
    }
}