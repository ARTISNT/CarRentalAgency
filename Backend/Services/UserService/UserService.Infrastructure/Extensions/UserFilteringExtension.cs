using UserService.Domain.Users;

namespace UserService.Infrastructure.Extensions;

public static class UserFilteringExtension
{
    public static IQueryable<User> ApplyFiltering(
        this IQueryable<User> query,
        UserSpecification specification)
    {
        if (specification.UserId.HasValue)
        {
            query = query.Where(u => u.Id == specification.UserId);
        }

        if (!string.IsNullOrWhiteSpace(specification.Role))
        {
            query = query.Where(u => u.Role.Name == specification.Role);
        }

        if (specification.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == specification.IsActive);
        }

        if (specification.IsEmailVerified.HasValue)
        {
            query = query.Where(u => u.EmailVerified == specification.IsEmailVerified);
        }

        if (!string.IsNullOrWhiteSpace(specification.Search))
        {
            query = query.Where(u =>
                u.Email.Value.Contains(specification.Search) ||
                u.PhoneNumber.Value.Contains(specification.Search));
        }

        query = query
            .OrderBy(u => u.Email.Value)
            .Skip((specification.Page - 1) * specification.PageSize)
            .Take(specification.PageSize);

        return query;
    }
}
