using MediatR;
using RentalService.Application.Abstractions.Security;
using RentalService.Domain.Payments;

namespace RentalService.Application.Features.Rentals.GetOutstandingFines;

public class GetOutstandingFinesQueryHandler(
    IPaymentRepository paymentRepository,
    IClientContext clientContext)
    : IRequestHandler<GetOutstandingFinesQuery, OutstandingFinesResponse>
{
    public async Task<OutstandingFinesResponse> Handle(
        GetOutstandingFinesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId != clientContext.ClientId)
        {
            var isStaff = clientContext.Permissions.Contains("ViewAllRents")
                          || clientContext.Permissions.Contains("EditRent");
            if (!isStaff)
                throw new UnauthorizedAccessException(
                    "Можно запрашивать только свои неоплаченные штрафы");
        }

        var amount = await paymentRepository.GetOutstandingFinesForRenterAsync(
            request.UserId,
            cancellationToken);

        return new OutstandingFinesResponse { OutstandingFines = amount };
    }
}
