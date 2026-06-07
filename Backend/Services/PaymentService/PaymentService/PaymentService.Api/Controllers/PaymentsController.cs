using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.PaymentMethods.Get;
using PaymentService.Application.Transactions.Create;
using PaymentService.Application.Transactions.Refund;
using PaymentService.Application.Transactions.Update;

namespace PaymentService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
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
        public async Task<IActionResult> PayAsync([FromRoute] Guid rentalId, [FromQuery] string type = "full")
        {
            var link = await _mediator.Send(new CreateTransactionCommand(rentalId, type));
            return Ok(link);
        }

        [HttpPost]
        [Route("refund/{rentalId}")]
        public async Task<IActionResult> RefundAsync([FromRoute] Guid rentalId)
        {
            await _mediator.Send(new RefundTransactionCommand(rentalId));
            return Ok();
        }

        [HttpPost]
        [Route("webhook")]
        public async Task<IActionResult> ProcessWebhookAsync()
        {
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            await _mediator.Send(new UpdateTransactionStatusCommand(json));

            return Ok();
        }
    }
}
