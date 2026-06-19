using AutoMapper;
using MediatR;
using UserService.Application.Exceptions;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.GetUserForRent;

public class GetUserForRentQueryHandler(IUserRepository userRepository, IMapper mapper)
    : IRequestHandler<GetUserForRentQuery, UserRentInfoResponse>
{
    public async Task<UserRentInfoResponse> Handle(GetUserForRentQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id) ??
                   throw new UserNotFoundException($"User with id {request.Id} not found");

        var response = mapper.Map<UserRentInfoResponse>(user);
        response.HasPassport = user.Passport is not null;
        return response;
    }
}