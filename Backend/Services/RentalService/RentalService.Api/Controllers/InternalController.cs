using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalService.Application.Features.Rentals.GetRentalForContract;
using RentalService.Application.Features.Rentals.GetRentalForPayment;

namespace RentalService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InternalController(ISender sender) : ControllerBase
{
    [HttpGet("get-rental-for-contract/{id}")]
    [Authorize(AuthenticationSchemes = "InternalAuth",  Policy = "ContractServiceOnly")]
    public async Task<IActionResult> GetRentalForContract(Guid id)
    {
        var rental = await sender.Send(new GetRentalForContractQuery(id));
        return Ok(rental);
    }

    [HttpGet("get-rental-for-payment/{id}")]
    [Authorize(AuthenticationSchemes = "InternalAuth", Policy = "PaymentServiceOnly")]
    public async Task<IActionResult> GetRentalForPayment(Guid id)
    {
        var rental = await sender.Send(new GetRentalForPaymentQuery(id));
        return Ok(rental);
    }
}