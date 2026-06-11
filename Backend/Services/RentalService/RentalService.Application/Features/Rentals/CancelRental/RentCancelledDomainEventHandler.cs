using Contracts.RentalEvents;
using RentalService.Application.Common;
using RentalService.Domain.DomainEvents;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.CancelRental;

public class RentCancelledDomainEventHandler(
    IRentalRepository rentalRepository,
    IIntegrationEventPublisher integrationEventPublisher)
    : IDomainEventHandler<RentCancelledDomainEvent>
{
    public async Task HandleAsync(RentCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(domainEvent.Id, cancellationToken);
        if (rental is null)
            return;

        await integrationEventPublisher.Publish(
            new RentalCancelledIntegrationEvent(
                rental.Id,
                rental.RentCarId,
                rental.CarRenterId,
                domainEvent.CancelledAt,
                domainEvent.Reason),
            cancellationToken);
    }
}
