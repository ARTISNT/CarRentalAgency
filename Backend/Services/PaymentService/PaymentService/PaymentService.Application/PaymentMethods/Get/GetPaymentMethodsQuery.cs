using MediatR;

namespace PaymentService.Application.PaymentMethods.Get
{
    public record GetPaymentMethodsQuery() : IRequest<IEnumerable<PaymentMethodDto>>;
}
