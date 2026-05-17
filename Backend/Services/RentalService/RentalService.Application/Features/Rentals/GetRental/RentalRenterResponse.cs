namespace RentalService.Application.Features.Rentals.GetRental;

public class RentalRenterResponse
{
    public string Name { get; set; } = null!;

    public string SurName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;
}