using ContractService.Domain.Common;

namespace ContractService.Domain.Contracts;

public record ContractTemplateSnapshot(
    int Version,
    string Name,
    string Content,
    DateTime ValidFrom,
    bool IsActive) : IValueObject;