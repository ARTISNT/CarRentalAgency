namespace UserService.Domain.Common;

public interface IPasswordProcessor
{
    string Hash(string password);
    string Verify(string hash, string password);
}  