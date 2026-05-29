namespace ContractService.Application.Abstractions.Security;

public interface IUserContext
{
    Guid UserId { get; }
}