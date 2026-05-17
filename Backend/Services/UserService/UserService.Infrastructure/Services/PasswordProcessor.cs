using Microsoft.AspNetCore.Identity;
using UserService.Domain.Common;
using UserService.Domain.Users;

namespace UserService.Infrastructure.Services;

public class PasswordProcessor(IPasswordHasher<User> passwordHasher) : IPasswordProcessor
{
    public string Hash(string password)
    {
        var hashedPassword = passwordHasher.HashPassword(null, password);
        return hashedPassword;
    }

    public bool Verify(string hash, string password)
    {
        var result = passwordHasher.VerifyHashedPassword(null, hash, password);
        return result == PasswordVerificationResult.Success;
    }
}