using AutoMapper;
using MediatR;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.GetUsers;
public class GetUserQueryHandler(IUserRepository userRepository, IMapper mapper) : 
    IRequestHandler<GetUsersQuery, IReadOnlyCollection<UserResponse>>
{
    public async Task<IReadOnlyCollection<UserResponse>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllUsersAsync(cancellationToken);
        if (!users!.Any())
            return Array.Empty<UserResponse>();
        
        return mapper.Map<IReadOnlyCollection<UserResponse>>(users);
    }
}