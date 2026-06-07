using PaymentService.Application.Abstractions.Repositories;
using MediatR;

namespace PaymentService.Application.PaymentMethods.Get
{
    public class GetPaymentMethodsQueryHandler : IRequestHandler<GetPaymentMethodsQuery, IEnumerable<PaymentMethodDto>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public GetPaymentMethodsQueryHandler(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<IEnumerable<PaymentMethodDto>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
        {
            var paymentMethods = await _paymentMethodRepository.GetAllAsync(cancellationToken);

            return paymentMethods.Select(pm => new PaymentMethodDto(pm.Name, pm.SystemName, pm.IsActive));
        }
    }
}
