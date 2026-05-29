using ContractService.Domain.Common;

namespace ContractService.Domain.Contracts;

public record ContractAutoSnapshot(
    string Brand,
    string Model,
    string CarBodyStyle,
    string LicensePlate,
    string Color) : IValueObject;