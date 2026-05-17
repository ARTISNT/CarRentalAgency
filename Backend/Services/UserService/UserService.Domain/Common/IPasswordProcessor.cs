namespace UserService.Domain.Common;

public interface IPasswordProcessor
{
    string Hash(string password);
    bool Verify(string hash, string password);
}  