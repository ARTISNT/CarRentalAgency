using ContractService.Domain.Common;

namespace ContractService.Domain.Contracts;

public record ClientSnapshot(
    string PhoneNumber,
    string PassportIdentificationNumber,
    string PassportNumber,
    string Name,
    string Surname,
    string Patronymic,
    DateTime PassportIssueDate,
    DateTime BirthDate) : IValueObject;