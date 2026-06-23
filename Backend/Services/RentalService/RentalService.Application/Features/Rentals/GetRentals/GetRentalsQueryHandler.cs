using AutoMapper;
using MediatR;
using RentalService.Application.Abstractions.Security;
using RentalService.Application.Authorization;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Features.Rentals.GetRentals;

public class GetRentalsQueryHandler(
    IRentalRepository rentalRepository,
    IMapper mapper,
    IClientContext clientContext,
    IRentalAuthorizationPolicy authorizationPolicy,
    IPaymentRepository paymentRepository) : IRequestHandler<GetRentalsQuery, IReadOnlyCollection<RentalListResponseDto>>
{
    public async Task<IReadOnlyCollection<RentalListResponseDto>> Handle(GetRentalsQuery request, CancellationToken cancellationToken)
    {
        if (!authorizationPolicy.CanViewAllRentals())
            request.RentalSpecification.CarRenterId = clientContext.ClientId;

        var rentals = await rentalRepository.GetRentalsAsync(request.RentalSpecification, cancellationToken);

        var responses = mapper.Map<IReadOnlyCollection<RentalListResponseDto>>(rentals);

        var rentalIds = rentals.Select(r => r.Id).ToList();
        var payments = await paymentRepository.GetPaymentsByRentIdsAsync(rentalIds, cancellationToken);
        var rentalsById = rentals.ToDictionary(r => r.Id);
        foreach (var dto in responses)
        {
            if (payments.TryGetValue(dto.Id, out var payment))
            {
                dto.TotalCost = payment.RequiredAmount.Amount;
                dto.Overpayment = payment.Overpayment.Amount;
                dto.AdditionalOutstanding = payment.AdditionalOutstanding.Amount;
            }
            if (rentalsById.TryGetValue(dto.Id, out var rental))
                dto.DepositRefundedAt = rental.DepositRefundedAt;
        }

        return responses;
    }
}