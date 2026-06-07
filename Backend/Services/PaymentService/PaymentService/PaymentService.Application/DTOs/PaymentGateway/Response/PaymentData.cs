namespace PaymentService.Application.DTOs.PaymentGateway.Response
{
    public class PaymentData
    {
        public string Token { get; set; } = default!;
        public string RedirectUrl { get; set; } = default!;
    }
}
