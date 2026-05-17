using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentalService.Api.Requests;
using RentalService.Application.Features.Rentals.CalculateEstimatedRentalPrice;
using RentalService.Application.Features.Rentals.CancelRental;
using RentalService.Application.Features.Rentals.CreateRental;
using RentalService.Application.Features.Rentals.EndRental;
using RentalService.Application.Features.Rentals.GetRental;
using RentalService.Application.Features.Rentals.GetRentals;
using RentalService.Application.Features.Rentals.RenewRental;

namespace RentalService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Route("GetRentals")]
    public async Task<IActionResult> GetRentals()
    {
        var rentals = await sender.Send(new GetRentalsQuery());
        return Ok(rentals);
    }
    
    [HttpGet]
    [Route("GetRental/{id}")]
    public async Task<IActionResult> GetRental([FromRoute] Guid id)
    {
        var rentals = await sender.Send(new GetRentalQuery(id));
        return Ok(rentals);
    }

    [HttpGet]
    [Route("CalculateEstimatedCost/{id}")]
    public async Task<IActionResult> CalculateEstimatedCost([FromRoute]Guid id)
    {
        var cost = await sender.Send(new GetEstimatedRentalPriceQuery(id));
        return Ok(cost);
    }
    
    [HttpPost]
    [Route("CreateRental")]
    public async Task<IActionResult> CreateRental([FromBody]CreateRentalRequest request)
    {
        await sender.Send(new CreateRentalCommand(request.UserId, request.CarId, 
            request.StartDate, request.EndDate));
        return Ok();
    }

    [HttpPut]
    [Route("RenewRental/{id}")]
    public async Task<IActionResult> RenewRental([FromBody] RenewRentalRequest renewRentalRequest, [FromRoute] Guid id)
    {
        await sender.Send(new RenewRentalCommand(id, renewRentalRequest.NewDate));
        return Ok();
    }

    [HttpPut]
    [Route("EndRental/{id}")]
    public async Task<IActionResult> EndRental([FromRoute] Guid id, [FromBody] EndRentalRequest endRentalRequest)
    {
        await sender.Send(new EndRentalCommand(id, endRentalRequest.ReturnDate, endRentalRequest.PromoCode));
        return Ok();
    }

    [HttpPut]
    [Route("CancelRental/{id}")]
    public async Task<IActionResult> CancelRental([FromRoute] Guid id, [FromBody]  CancelRentalRequest cancelRentalRequest)
    {
        await sender.Send(new CancelRentalCommand(id, DateTime.UtcNow));
        return Ok();
    }
}