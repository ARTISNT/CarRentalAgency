using PaymentService.Application.DTOs.Rentals;

namespace PaymentService.Application.Abstractions.Clients
{
    public interface IRentalServiceClient
    {
        Task<RentalDto> GetRentalByIdAsync(Guid rentalId);
    }
}
