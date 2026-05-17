using AutoMapper;
using MediatR;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.AddUserPassport;

public class AddUserPassportCommandHandler(IUserRepository userRepository, IMapper mapper)
    : IRequestHandler<AddUserPassportCommand>
{
    public async Task Handle(AddUserPassportCommand request, CancellationToken cancellationToken)
    {
        var passportDto = request.PassportRequest;
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken) ??
                   throw new KeyNotFoundException("User not found");
        
        var passport = new Passport(
            passportDto.PassportNumber,
            passportDto.IdentityNumber,
            passportDto.Name, 
            passportDto.Surname,
            passportDto.Patronymic,
            passportDto.PassportIssueDate,
            passportDto.BirthDate);
        
        user.AddPassport(passport);
        await userRepository.UpdateAsync(user, cancellationToken);
    }
}