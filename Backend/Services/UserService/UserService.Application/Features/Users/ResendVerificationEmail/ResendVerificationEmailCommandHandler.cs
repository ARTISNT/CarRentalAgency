using MediatR;
using UserService.Domain.Users;

namespace UserService.Application.Features.Users.ResendVerificationEmail;

public class ResendVerificationEmailCommandHandler(
    IUserRepository userRepository,
    ISender sender)
    : IRequestHandler<ResendVerificationEmailCommand, ResendVerificationEmailResult>
{
    public async Task<ResendVerificationEmailResult> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
            return ResendVerificationEmailResult.UserNotFound;

        if (user.EmailVerified)
            return ResendVerificationEmailResult.AlreadyVerified;

        await sender.Send(new RequestEmailVerification.RequestEmailVerificationCommand(user.Id), cancellationToken);
        return ResendVerificationEmailResult.Sent;
    }
}
