using RentalService.Domain.Rentals.Enums;

namespace RentalService.Application.Features.Rentals.GetRental;

public class RentalResponse
{
    public Guid Id { get; set; }

    public RentActivityStatus ActivityStatus { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public DateTime? ReturnRequestedAtUtc { get; set; }
    public DateTime? DepositRefundedAt { get; set; }

    public decimal TotalCost { get; set; }

    public decimal DepositAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RequiredAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;

    public decimal FineOutstanding { get; set; }

    public Guid CarRenterId { get; set; }

    public RentalCarResponse Car { get; set; } = null!;

    public RentalRenterResponse Renter { get; set; } = null!; 
}