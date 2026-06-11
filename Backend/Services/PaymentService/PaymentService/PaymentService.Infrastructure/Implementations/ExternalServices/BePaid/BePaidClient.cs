using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using PaymentService.Application.Abstractions.PaymentGateway;
using PaymentService.Application.DTOs.PaymentGateway.Response;
using PaymentService.Infrastructure.Implementations.ExternalServices.BePaid.DTOs.Response;

namespace PaymentService.Infrastructure.Implementations.ExternalServices.BePaid
{
    public class BePaidClient : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public BePaidClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public string BuildUrl(string token)
        {
            return $"https://checkout.bepaid.by/v2/checkout?token={token}";
        }

        public async Task<PaymentData> CreateSessionsAsync(decimal amount, string trackingId)
        {
            return await CreateSessionsAsync(amount, trackingId, "Оплата аренды авто");
        }

        public async Task<PaymentData> CreateSessionsAsync(decimal amount, string trackingId, string description)
        {
            var shopId = _configuration["BePaid:ShopId"];
            var secretKey = _configuration["BePaid:SecretKey"];
            var basicAuth = Encoding.UTF8.GetBytes($"{shopId}:{secretKey}");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://checkout.bepaid.by/ctp/api/checkouts");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(basicAuth));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("X-API-Version", "2");

            var bePaidRequest = new
            {
                checkout = new
                {
                    test = true,
                    transaction_type = "payment",
                    attempts = 3,
                    iframe = true,
                    order = new
                    {
                        currency = "BYN",
                        amount = (int)(amount * 100),
                        description = description,
                        tracking_id = trackingId,
                        additional_data = new
                        {
                            receipt_text = new[] { $"{description} #{trackingId}" }
                        }
                    },
                    settings = new
                    {
                        return_url = _configuration["BePaid:CallbackUrl"] ?? "http://localhost:5173/payment/callback",
                        success_url = _configuration["BePaid:CallbackUrl"] ?? "http://localhost:5173/payment/callback",
                        decline_url = "https://bepaid.by",
                        fail_url = "https://bepaid.by",
                        cancel_url = "https://bepaid.by",
                        notification_url = _configuration["BePaid:NotificationUrl"] ?? "",
                        button_next_text = "Вернуться в магазин",
                        auto_return = "0",
                        language = "ru",
                        customer_fields = new
                        {
                            visible = new[] { "first_name", "last_name" }
                        },
                        payment_method = new
                        {
                            types = new[] { "credit_card" }
                        },
                        customer = new
                        {
                            first_name = "",
                            last_name = "",
                            address = "",
                            country = "Belarus",
                            city = ""
                        }
                    }
                }
            };

            request.Content = JsonContent.Create(bePaidRequest);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error occurred: {error}");
            }

            var data = await response.Content.ReadFromJsonAsync<BePaidResponse>();

            return new PaymentData
            {
                Token = data!.Checkout.Token,
                RedirectUrl = data.Checkout.RedirectUrl
            };
        }

        public async Task<string> RefundAsync(string token, decimal amount)
        {
            var shopId = _configuration["BePaid:ShopId"];
            var secretKey = _configuration["BePaid:SecretKey"];
            var basicAuth = Encoding.UTF8.GetBytes($"{shopId}:{secretKey}");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://gateway.bepaid.by/transactions/refunds");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(basicAuth));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("X-API-Version", "2");

            var refundRequest = new
            {
                request = new
                {
                    parent_uid = token,
                    amount = (int)(amount * 100),
                    reason = "deposit_refund"
                }
            };

            request.Content = JsonContent.Create(refundRequest);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Refund error: {error}");
            }

            var refundResponse = await response.Content.ReadFromJsonAsync<BePaidRefundResponse>();
            return refundResponse!.Transaction.Uid;
        }
    }

    public class BePaidRefundResponse
    {
        public BePaidRefundTransaction Transaction { get; set; } = default!;
    }

    public class BePaidRefundTransaction
    {
        public string Uid { get; set; } = default!;
    }
}
