using RentalService.Domain.Rentals.Enums;

namespace RentalService.Application.Features.Rentals.GetRentals;

public class RentalListResponseDto
{
    public Guid Id { get; set; }

    public string Car { get; set; } = null!;

    public string Renter { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public RentActivityStatus ActivityStatus { get; set; }

    public decimal TotalCost { get; set; }
}