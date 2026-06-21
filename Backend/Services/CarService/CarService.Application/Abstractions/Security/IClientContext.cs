namespace CarService.Application.Abstractions.Security;

public interface IClientContext
{
    Guid ClientId { get; }
    string[] Permissions { get; }
    bool? IsActive { get; }
}
