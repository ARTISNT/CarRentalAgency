using MediatR;

namespace RentalService.Application.Features.Rentals.PreviewFinalCost;

public record PreviewFinalCostQuery(Guid Id, DateTime ReturnDate) : IRequest<PreviewFinalCostResponse>;
