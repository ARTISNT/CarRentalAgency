using CarService.Application.Features.AddCar;
using CarService.Application.Features.GetCarForRent;
using CarService.Application.Features.GetCars;
using CarService.Application.Features.GetDetailedCars;
using CarService.Application.Features.UpdateCar;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCars()
    {
        var cars = await sender.Send(new GetCarsQuery());
        return Ok(cars);
    }

    [HttpGet("get-car-for-rent/{id}")]
    public async Task<IActionResult> GetCarForRent(Guid id)
    {
        var car = await sender.Send(new GetCarForRentQuery(id));
        return Ok(car);
    }
    
    [HttpGet]
    [Route("detailed-car/{carId}")]
    public async Task<IActionResult> GetDetailedCarById(Guid carId)
    {
        var cars = await sender.Send(new GetDetailedCarQuery(carId));
        return Ok(cars);
    }

    [HttpPost]
    [Route("add-car")]
    public async Task<IActionResult> CreateCar([FromBody]CreateCarDto carDto)
    {
        await sender.Send(new AddCarCommand(carDto));
        return Ok();
    }

    [HttpPut]
    [Route("update-car/{id}")]
    public async Task<IActionResult> UpdateCar([FromRoute] Guid id,[FromBody] UpdateCarDto carDto)
    {
        await sender.Send(new UpdateCarCommand(id, carDto));
        return Ok();
    }
}