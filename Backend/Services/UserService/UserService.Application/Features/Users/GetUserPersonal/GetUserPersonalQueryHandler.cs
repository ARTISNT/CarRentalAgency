using AutoMapper;
using MediatR;
using UserService.Application.Exceptions;
using UserService.Application.Features.Users.GetUsers;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.GetUserPersonal;

public class GetUserPersonalQueryHandler(IUserRepository userRepository,
    IMapper mapper) : IRequestHandler<GetUserPersonalQuery, UserResponseWithPassport>
{
    public async Task<UserResponseWithPassport> Handle(GetUserPersonalQuery request, CancellationToken cancellationToken)
    {
        var userWithPersonality = await userRepository.GetByIdAsync(request.Id, cancellationToken) ?? 
                                  throw new UserNotFoundException($"User with id {request.Id}  not found");
        
        return mapper.Map<UserResponseWithPassport>(userWithPersonality);
    }
}