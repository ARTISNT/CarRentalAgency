using AutoMapper;
using CarService.Api.Requests;
using CarService.Application.Features.AddCar;
using CarService.Application.Features.GetCars;
using CarService.Application.Features.GetDetailedCars;
using CarService.Application.Features.RemoveCar;
using CarService.Application.Features.UpdateCar;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarController(ISender sender, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCars(CancellationToken cancellationToken)
    {
        var cars = await sender.Send(new GetCarsQuery(), cancellationToken);
        return Ok(cars);
    }
    
    [HttpGet]
    [Route("detailed-car/{carId}")]
    public async Task<IActionResult> GetDetailedCarById([FromRoute]Guid carId, CancellationToken cancellationToken)
    {
        var cars = await sender.Send(new GetDetailedCarQuery(carId), cancellationToken);
        return Ok(cars);
    }

    [HttpPost]
    [Route("add-car")]
    public async Task<IActionResult> CreateCar([FromBody]CreateCarRequest carRequest, CancellationToken cancellationToken)
    {
        await sender.Send(mapper.Map<AddCarCommand>(carRequest), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("update-car/{id}")]
    public async Task<IActionResult> UpdateCar([FromRoute] Guid id,[FromBody] UpdateCarRequests carRequests, CancellationToken cancellationToken)
    {
        await sender.Send(mapper.Map<UpdateCarCommand>((id, carRequests)), cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("delete-car/{id}")]
    public async Task<IActionResult> DeleteCar([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveCarCommand(id), cancellationToken);
        return Ok();
    }
}