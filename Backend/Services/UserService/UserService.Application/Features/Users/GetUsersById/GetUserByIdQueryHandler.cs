using AutoMapper;
using MediatR;
using UserService.Application.Exceptions;
using UserService.Application.Features.Users.GetUsers;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.GetUsersById;

public class GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper) 
    : IRequestHandler<GetUserByIdQuery,UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(query.UserId, cancellationToken);
        if(user is null)
            throw new UserNotFoundException($"User with id {query.UserId} not found");
        
        return mapper.Map<UserResponse>(user);
    }
}