using Contracts.RentalEvents;
using RentalService.Application.Common;
using RentalService.Domain.DomainEvents;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.StartRental;

public class RentStartedDomainEventHandler(
    IRentalRepository rentalRepository,
    IIntegrationEventPublisher integrationEventPublisher)
    : IDomainEventHandler<RentStartedDomainEvent>
{
    public async Task HandleAsync(RentStartedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var rental = await rentalRepository.GetRentalAsync(domainEvent.Id, cancellationToken);
        if (rental is null)
            return;

        await integrationEventPublisher.Publish(
            new RentalStartedIntegrationEvent(
                rental.Id,
                rental.CarRenterId,
                rental.RentCarId,
                DateTime.UtcNow),
            cancellationToken);
    }
}
