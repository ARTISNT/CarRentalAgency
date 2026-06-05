using System.Diagnostics;
using RentalService.Application.Common;
using RentalService.Domain.DomainEvents;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.CreateRental;

public class CreateRentDomainEventHandler(
    IRentalRepository rentalRepository,
    IPaymentRepository paymentRepository,
    IIntegrationEventPublisher integrationEventPublisher) 
    : IDomainEventHandler<RentCreatedDomainEvent>
{
    public async Task HandleAsync(RentCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(domainEvent.Id, cancellationToken) ??
                     throw new KeyNotFoundException("Rental not found");

        
    }
}