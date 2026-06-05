using MediatR;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.GetRentalForContract;

public class GetRentalForContractQueryHandler(
    IRentalRepository rentalRepository,
    IPaymentRepository paymentRepository) 
    : IRequestHandler<GetRentalForContractQuery, RentalForContractResponse>
{
    public async Task<RentalForContractResponse> Handle(GetRentalForContractQuery request, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(request.Id, cancellationToken)
                     ?? throw new KeyNotFoundException("Rental not found");
        
        var payment = await paymentRepository.GetPaymentByRentIdAsync(rental.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Payment not found");

        var rentalForContract = new RentalForContractResponse()
        {
            RentalId = rental.Id,
            EndDate = rental.EndDate,
            StartDate = rental.StartDate,
            EstimatedPrice= payment.EstimatedAmount.Amount
        };
        
        return rentalForContract;
    }
}