using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalService.Api.Requests;
using RentalService.Application.Common;
using RentalService.Application.Features.Rentals.CalculateEstimatedRentalPrice;
using RentalService.Application.Features.Rentals.CancelRental;
using RentalService.Application.Features.Rentals.CreateRental;
using RentalService.Application.Features.Rentals.EndRental;
using RentalService.Application.Features.Rentals.GetRental;
using RentalService.Application.Features.Rentals.GetRentals;
using RentalService.Application.Features.Rentals.PreviewFinalCost;
using RentalService.Application.Features.Rentals.RenewRental;
using RentalService.Application.Features.Rentals.RequestReturnRental;
using RentalService.Domain.Rentals;

namespace RentalService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Route("GetRentals")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.ViewRents)]
    public async Task<IActionResult> GetRentals([FromQuery] RentalSpecification specification)
    {
        var rentals = await sender.Send(new GetRentalsQuery(specification));
        return Ok(rentals);
    }

    [HttpGet]
    [Route("PreviewFinalCost/{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.ViewRents)]
    public async Task<IActionResult> PreviewFinalCost(
        [FromRoute] Guid id,
        [FromQuery] DateTime returnDate)
    {
        var result = await sender.Send(new PreviewFinalCostQuery(id, returnDate));
        return Ok(result);
    }
    
    [HttpGet]
    [Route("GetRental/{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.ViewRents)]
    public async Task<IActionResult> GetRental([FromRoute] Guid id)
    {
        var rentals = await sender.Send(new GetRentalQuery(id));
        return Ok(rentals);
    }

    [HttpPost]
    [Route("CalculateEstimatedCost/{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.ViewRents)]
    public async Task<IActionResult> CalculateEstimatedCost([FromRoute]Guid id, [FromBody]GetEstimatedRentalPriceRequest request)
    {
        var cost = await sender.Send(new GetEstimatedRentalPriceQuery(id, request.PromoCode));
        return Ok(cost);
    }
    
    [HttpPost]
    [Route("CreateRental")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.CreateRent)]
    public async Task<IActionResult> CreateRental([FromBody]CreateRentalRequest request)
    {
        var rentalId = await sender.Send(new CreateRentalCommand(request.UserId, request.CarId, 
            request.StartDate, request.EndDate, request.PromoCode));
        return Ok(new { rentalId });
    }

    [HttpPut]
    [Route("RenewRental/{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.EditRent)]
    public async Task<IActionResult> RenewRental([FromBody] RenewRentalRequest renewRentalRequest, [FromRoute] Guid id)
    {
        await sender.Send(new RenewRentalCommand(id, renewRentalRequest.NewDate));
        return Ok();
    }

    [HttpPut]
    [Route("EndRental/{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.EditRent)]
    public async Task<IActionResult> EndRental([FromRoute] Guid id, [FromBody] EndRentalRequest endRentalRequest)
    {
        await sender.Send(new EndRentalCommand(
            id,
            endRentalRequest.ReturnDate,
            endRentalRequest.Mileage,
            endRentalRequest.FuelLevel,
            endRentalRequest.PenaltyAmount,
            endRentalRequest.DamageDescription));
        return Ok();
    }

    [HttpPost]
    [Route("RequestReturn/{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.ViewRents)]
    public async Task<IActionResult> RequestReturn([FromRoute] Guid id)
    {
        await sender.Send(new RequestReturnCommand(id));
        return Ok();
    }

    [HttpPut]
    [Route("CancelRental/{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.EditRent)]
    public async Task<IActionResult> CancelRental([FromRoute] Guid id, [FromBody]  CancelRentalRequest cancelRentalRequest)
    {
        await sender.Send(new CancelRentalCommand(id, DateTime.UtcNow));
        return Ok();
    }
}