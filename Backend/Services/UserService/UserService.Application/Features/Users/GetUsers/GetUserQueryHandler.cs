using AutoMapper;
using MediatR;
using UserService.Application.Common;
using UserService.Domain.Users;
using Permission = UserService.Domain.Permissions.Permission;

namespace UserService.Application.Features.Users.GetUsers;
public class GetUserQueryHandler(
    IUserRepository userRepository,
    IMapper mapper,
    IUserContext userContext) : 
    IRequestHandler<GetUsersQuery, IReadOnlyCollection<UserResponse>>
{
    public async Task<IReadOnlyCollection<UserResponse>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var currentUser = await userRepository.GetByIdAsync(userContext.UserId, cancellationToken);

        if (currentUser is not null && !currentUser.Role.HasPermission(Permission.ViewUsers))
            query.UserSpecification.UserId = currentUser.Id;

        var users = await userRepository.GetAllUsersAsync(query.UserSpecification, cancellationToken);
        if (users is null || !users.Any())
            return Array.Empty<UserResponse>();
        
        return mapper.Map<IReadOnlyCollection<UserResponse>>(users);
    }
}