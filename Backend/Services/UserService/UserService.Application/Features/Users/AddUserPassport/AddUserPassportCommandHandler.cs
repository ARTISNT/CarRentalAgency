using AutoMapper;
using MediatR;
using UserService.Application.Authorization;
using UserService.Application.Common;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.AddUserPassport;

public class AddUserPassportCommandHandler(
    IUserRepository userRepository, 
    IUserAuthorizationService userAuthorizationService, 
    IMapper mapper)
    : IRequestHandler<AddUserPassportCommand>
{
    public async Task Handle(AddUserPassportCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken) ??
                   throw new KeyNotFoundException("User not found");
        
        await userAuthorizationService.EnsureCanEditUser(user.Id, cancellationToken);
        
        var passport = new Passport(
            request.PassportNumber,
            request.IdentityNumber,
            request.Name, 
            request.Surname,
            request.Patronymic,
            request.PassportIssueDate,
            request.BirthDate);
        
        user.AddPassport(passport);
        await userRepository.UpdateAsync(user, cancellationToken);
    }
}