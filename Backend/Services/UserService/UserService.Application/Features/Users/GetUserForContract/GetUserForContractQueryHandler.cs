using AutoMapper;
using MediatR;
using UserService.Application.Exceptions;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.GetUserForContract;

public class GetUserForContractQueryHandler(
    IUserRepository userRepository,
    IMapper mapper)
    : IRequestHandler<GetUserForContractQuery, ClientForContractResponse>
{
    public async Task<ClientForContractResponse> Handle(GetUserForContractQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new UserNotFoundException("User not found");
        
        return mapper.Map<ClientForContractResponse>(user);
    }
}