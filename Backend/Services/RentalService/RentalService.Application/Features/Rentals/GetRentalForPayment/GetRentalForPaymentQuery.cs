using MediatR;

namespace RentalService.Application.Features.Rentals.GetRentalForPayment;

public record GetRentalForPaymentQuery(Guid Id) : IRequest<RentalForPaymentResponse>;
