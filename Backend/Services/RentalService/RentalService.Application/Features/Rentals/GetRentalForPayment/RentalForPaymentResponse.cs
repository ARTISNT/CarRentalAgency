namespace RentalService.Application.Features.Rentals.GetRentalForPayment;

public class RentalForPaymentResponse
{
    public Guid RentalId { get; set; }
    public string UserName { get; set; } = default!;
    public string UserPhone { get; set; } = default!;
    public string CarName { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }
}
