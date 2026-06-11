using AutoMapper;
using CarService.Api.Requests;
using CarService.Application.Common;
using CarService.Application.Features.AddCar;
using CarService.Domain.Cars;
using CarService.Application.Features.BreakCar;
using CarService.Application.Features.CompleteMaintenance;
using CarService.Application.Features.GetAvailableCars;
using CarService.Application.Features.GetMyRentedCars;
using CarService.Application.Features.GetPublicDetailedCar;
using CarService.Application.Features.GetCars;
using CarService.Application.Features.GetDetailedCars;
using CarService.Application.Features.MarkCarAsReturned;
using CarService.Application.Features.ProcessReturnedCar;
using CarService.Application.Features.RemoveCar;
using CarService.Application.Features.RentCar;
using CarService.Application.Features.ReturnCar;
using CarService.Application.Features.SendCarToMaintenance;
using CarService.Application.Features.SendCarToRepair;
using CarService.Application.Features.UpdateCar;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarController(ISender sender, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = "ViewCars")]
    public async Task<IActionResult> GetCars([FromQuery] CarSpecification specification, CancellationToken cancellationToken)
    {
        var cars = await sender.Send(new GetCarsQuery(specification), cancellationToken);
        return Ok(cars);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableCars([FromQuery] CarSpecification specification, CancellationToken cancellationToken)
    {
        var cars = await sender.Send(new GetAvailableCarsQuery(specification), cancellationToken);
        return Ok(cars);
    }

    [HttpGet("public-car/{carId}")]
    public async Task<IActionResult> GetPublicDetailedCar([FromRoute] Guid carId, CancellationToken cancellationToken)
    {
        var car = await sender.Send(new GetPublicDetailedCarQuery(carId), cancellationToken);
        return Ok(car);
    }

    [HttpGet("my-rented")]
    [Authorize(AuthenticationSchemes = "UserAuth")]
    public async Task<IActionResult> GetMyRentedCars([FromQuery] CarSpecification specification, CancellationToken cancellationToken)
    {
        var cars = await sender.Send(new GetMyRentedCarsQuery(specification), cancellationToken);
        return Ok(cars);
    }
    
    [HttpGet]
    [Route("detailed-car/{carId}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = "ViewCars")]
    public async Task<IActionResult> GetDetailedCarById([FromRoute]Guid carId, CancellationToken cancellationToken)
    {
        var cars = await sender.Send(new GetDetailedCarQuery(carId), cancellationToken);
        return Ok(cars);
    }

    [HttpPost]
    [Route("add-car")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = "CreateCars")]
    public async Task<IActionResult> CreateCar([FromBody]CreateCarRequest carRequest, CancellationToken cancellationToken)
    {
        await sender.Send(mapper.Map<AddCarCommand>(carRequest), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("update-car/{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = "EditCarsDetails")]
    public async Task<IActionResult> UpdateCar([FromRoute] Guid id,[FromBody] UpdateCarRequests carRequests, CancellationToken cancellationToken)
    {
        await sender.Send(mapper.Map<UpdateCarCommand>((id, carRequests)), cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("delete-car/{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = "DeleteCars")]
    public async Task<IActionResult> DeleteCar([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveCarCommand(id), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("rent/{carId}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.UpdateCars)]
    public async Task<IActionResult> RentCar([FromRoute] Guid carId, CancellationToken cancellationToken)
    {
        await sender.Send(new RentCarCommand(carId), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("return/{carId}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.UpdateCars)]
    public async Task<IActionResult> ReturnCar([FromRoute] Guid carId, CancellationToken cancellationToken)
    {
        await sender.Send(new ReturnCarCommand(carId), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("mark-returned/{carId}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.UpdateCars)]
    public async Task<IActionResult> MarkCarAsReturned([FromRoute] Guid carId, CancellationToken cancellationToken)
    {
        await sender.Send(new MarkCarAsReturnedCommand(carId), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("break/{carId}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.UpdateCars)]
    public async Task<IActionResult> BreakCar([FromRoute] Guid carId, CancellationToken cancellationToken)
    {
        await sender.Send(new BreakCarCommand(carId), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("send-to-maintenance/{carId}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.UpdateCars)]
    public async Task<IActionResult> SendCarToMaintenance([FromRoute] Guid carId, CancellationToken cancellationToken)
    {
        await sender.Send(new SendCarToMaintenanceCommand(carId), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("send-to-repair/{carId}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.UpdateCars)]
    public async Task<IActionResult> SendCarToRepair([FromRoute] Guid carId, CancellationToken cancellationToken)
    {
        await sender.Send(new SendCarToRepairCommand(carId), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("complete-maintenance/{carId}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.UpdateCars)]
    public async Task<IActionResult> CompleteMaintenance([FromRoute] Guid carId, CancellationToken cancellationToken)
    {
        await sender.Send(new CompleteMaintenanceCommand(carId), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("process-return/{carId}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = "ProcessCarReturn")]
    public async Task<IActionResult> ProcessReturn(
        [FromRoute] Guid carId,
        [FromBody] ProcessReturnRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new ProcessReturnedCarCommand(carId, request.TargetStatus), cancellationToken);
        return Ok();
    }
}
