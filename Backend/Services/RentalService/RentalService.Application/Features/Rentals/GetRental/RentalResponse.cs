using RentalService.Domain.Rentals.Enums;

namespace RentalService.Application.Features.Rentals.GetRental;

public class RentalResponse
{
    public Guid Id { get; set; }

    public RentActivityStatus ActivityStatus { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public decimal TotalCost { get; set; }

    public RentalCarResponse Car { get; set; } = null!;

    public RentalRenterResponse Renter { get; set; } = null!; 
}