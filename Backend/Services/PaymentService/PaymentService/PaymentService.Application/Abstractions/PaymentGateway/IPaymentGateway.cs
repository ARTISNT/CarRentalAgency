using PaymentService.Application.DTOs.PaymentGateway.Response;

namespace PaymentService.Application.Abstractions.PaymentGateway
{
    public interface IPaymentGateway
    {
        Task<PaymentData> CreateSessionsAsync(decimal amount, string trackingId);
        Task<string> RefundAsync(string token, decimal amount);
        string BuildUrl(string token);
    }
}
