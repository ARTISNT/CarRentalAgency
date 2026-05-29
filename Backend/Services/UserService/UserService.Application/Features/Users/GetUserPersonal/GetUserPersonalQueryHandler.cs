using AutoMapper;
using MediatR;
using UserService.Application.Authorization;
using UserService.Application.Exceptions;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.GetUserPersonal;

public class GetUserPersonalQueryHandler(
    IUserRepository userRepository,
    IMapper mapper, 
    IUserAuthorizationService userAuthorizationService)
    : IRequestHandler<GetUserPersonalQuery, UserResponseWithPassport>
{
    public async Task<UserResponseWithPassport> Handle(GetUserPersonalQuery request, CancellationToken cancellationToken)
    {
        var userWithPersonality = await userRepository.GetByIdAsync(request.Id, cancellationToken) ?? 
                                  throw new UserNotFoundException($"User with id {request.Id}  not found");
        
        await userAuthorizationService.EnsureCanViewUser(userWithPersonality.Id, cancellationToken);
        return mapper.Map<UserResponseWithPassport>(userWithPersonality);
    }
}