namespace ContractService.Application.Features.Contracts.GetDetailedContract;

public class ClientResponse
{
    public string PhoneNumber { get; init; }
    public string PassportIdentificationNumber { get; init; }
    public string PassportNumber { get; init; }
    public string Name { get; init; }
    public string Surname { get; init; }
    public string Patronymic { get; init; }
    public DateTime PassportIssueDate { get; init; }
    public DateTime BirthDate { get; init; }
}