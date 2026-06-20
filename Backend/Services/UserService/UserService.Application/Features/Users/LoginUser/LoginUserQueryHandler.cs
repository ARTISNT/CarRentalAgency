using MediatR;
using UserService.Application.Common;
using UserService.Domain.Common;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.LoginUser;

public class LoginUserQueryHandler(
    IUserRepository userRepository,
    IJwtProvider jwtProvider,
    IPasswordProcessor passwordProcessor)
    : IRequestHandler<LoginUserQuery, string>
{
    public async Task<string> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken) ??
                   throw new KeyNotFoundException("User not found");

        if (!passwordProcessor.Verify(user.Password.Hash, request.Password))
            throw new UnauthorizedAccessException("Invalid password");

        if (!user.EmailVerified)
            throw new EmailNotVerifiedException("Email is not verified.");

        var token = jwtProvider.CreateJwtToken(user);
        return token;
    }
}
