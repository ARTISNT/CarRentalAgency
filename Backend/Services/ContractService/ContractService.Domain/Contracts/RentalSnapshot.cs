using ContractService.Domain.Common;

namespace ContractService.Domain.Contracts;

public record RentalSnapshot(
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalPrice) : IValueObject;