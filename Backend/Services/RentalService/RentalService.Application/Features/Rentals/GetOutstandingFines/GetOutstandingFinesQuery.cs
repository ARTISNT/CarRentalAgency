using MediatR;

namespace RentalService.Application.Features.Rentals.GetOutstandingFines;

public record GetOutstandingFinesQuery(Guid UserId) : IRequest<OutstandingFinesResponse>;
