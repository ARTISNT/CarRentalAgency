namespace UserService.Domain.Users;

public interface IUserRepository
{ 
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<User>?> GetAllUsersAsync(UserSpecification userSpecification, CancellationToken cancellationToken = default);
    
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task RemoveAsync(User user, CancellationToken cancellationToken = default); 
}
