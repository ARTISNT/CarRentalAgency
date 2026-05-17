using RentalService.Domain.Common;

namespace RentalService.Domain.Rentals;

public record CarRenterSnapshot(
    string Name,
    string SurName,
    string Patronymic,
    string PhoneNumber, 
    string Email
    ) : IValueObject;