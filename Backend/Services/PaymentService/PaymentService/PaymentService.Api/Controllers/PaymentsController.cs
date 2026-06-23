using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.PaymentMethods.Get;
using PaymentService.Application.Transactions.Confirm;
using PaymentService.Application.Transactions.Create;
using PaymentService.Application.Transactions.CreateAdditional;
using PaymentService.Application.Transactions.CreateFine;
using PaymentService.Application.Transactions.CreateRemaining;
using PaymentService.Application.Transactions.GetByRental;
using PaymentService.Application.Transactions.Refund;
using PaymentService.Application.Transactions.Update;
using System.Security.Cryptography;
using System.Text;

namespace PaymentService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IMediator mediator, IConfiguration configuration, ILogger<PaymentsController> logger)
        {
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        [Route("methods")]
        public async Task<IActionResult> GetPaymentMethodsAsync()
        {
            var results = await _mediator.Send(new GetPaymentMethodsQuery());
            return Ok(results);
        }

        [HttpPost]
        [Route("pay/{rentalId}")]
        public async Task<IActionResult> PayAsync([FromRoute] Guid rentalId, [FromQuery] string type = "FullPayment")
        {
            var link = await _mediator.Send(new CreateTransactionCommand(rentalId, type));
            return Ok(link);
        }

        [HttpPost]
        [Route("pay-fine/{rentalId}")]
        public async Task<IActionResult> PayFineAsync(
            [FromRoute] Guid rentalId,
            [FromBody] PayFineRequest request)
        {
            var link = await _mediator.Send(new CreateFinePaymentCommand(rentalId, request.Amount, request.Reason));
            return Ok(link);
        }

        [HttpPost]
        [Route("pay-additional/{rentalId}")]
        public async Task<IActionResult> PayAdditionalAsync(
            [FromRoute] Guid rentalId,
            [FromBody] PayAdditionalRequest request)
        {
            var link = await _mediator.Send(new CreateAdditionalPaymentCommand(rentalId, request.Amount, request.Reason));
            return Ok(link);
        }

        [HttpPost]
        [Route("pay-remaining/{rentalId}")]
        public async Task<IActionResult> PayRemainingAsync([FromRoute] Guid rentalId)
        {
            var link = await _mediator.Send(new CreateRemainingPaymentCommand(rentalId));
            return Ok(link);
        }

        [HttpPost]
        [Route("refund/{rentalId}")]
        public async Task<IActionResult> RefundAsync([FromRoute] Guid rentalId)
        {
            await _mediator.Send(new RefundTransactionCommand(rentalId));
            return Ok();
        }

        [HttpGet]
        [Route("transactions/by-rental/{rentalId}")]
        public async Task<IActionResult> GetTransactionsByRentalAsync([FromRoute] Guid rentalId)
        {
            var transactions = await _mediator.Send(new GetTransactionsByRentalQuery(rentalId));
            return Ok(transactions);
        }

        [HttpGet]
        [Route("payment-summary/{rentalId}")]
        public async Task<IActionResult> GetPaymentSummaryAsync([FromRoute] Guid rentalId)
        {
            var summary = await _mediator.Send(new GetPaymentSummaryQuery(rentalId));
            return Ok(summary);
        }

        [HttpPost]
        [Route("webhook")]
        public async Task<IActionResult> ProcessWebhookAsync()
        {
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            var webhookSecret = _configuration["BePaid:WebhookSecret"];
            if (!string.IsNullOrEmpty(webhookSecret))
            {
                if (!Request.Headers.TryGetValue("X-BePaid-Signature", out var sig) || string.IsNullOrEmpty(sig))
                {
                    _logger.LogWarning("BePaid webhook rejected: missing X-BePaid-Signature header");
                    return Unauthorized(new { error = "missing_signature" });
                }

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(json));
                var expected = Convert.ToHexString(hash).ToLowerInvariant();
                if (!string.Equals(expected, sig.ToString().ToLowerInvariant(), StringComparison.Ordinal))
                {
                    _logger.LogWarning("BePaid webhook rejected: invalid signature");
                    return Unauthorized(new { error = "invalid_signature" });
                }
            }
            else
            {
                _logger.LogWarning("BePaid:WebhookSecret is not configured; webhook is processed without signature validation");
            }

            await _mediator.Send(new UpdateTransactionStatusCommand(json));

            return Ok();
        }

        [HttpPost]
        [Route("confirm")]
        public async Task<IActionResult> ConfirmAsync([FromQuery] string token)
        {
            var rentalId = await _mediator.Send(new ConfirmPaymentCommand(token));
            return Ok(new { rentalId });
        }
    }

    public class PayFineRequest
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; } = "Penalty";
    }

    public class PayAdditionalRequest
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; } = "Продление аренды";
    }
}
