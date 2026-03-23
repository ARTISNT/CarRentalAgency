namespace UserService.Domain.Users;

public interface IUserRepository
{ 
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetAllUsersAsync(CancellationToken cancellationToken = default);
    
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Remove(Guid userId); 
}
