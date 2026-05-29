using CarService.Application.Features.GetCarForContract;
using CarService.Application.Features.GetCarForRent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InternalController(ISender sender) : ControllerBase
{
    [HttpGet("get-car-for-rent/{id}")]
    [Authorize(AuthenticationSchemes = "InternalAuth", Policy = "RentalServiceOnly")]
    public async Task<IActionResult> GetCarForRent(Guid id)
    {
        var car = await sender.Send(new GetCarForRentQuery(id));
        return Ok(car);
    }
    
    [HttpGet("get-car-for-contract/{id}")]
    [Authorize(AuthenticationSchemes = "InternalAuth", Policy = "ContractServiceOnly")]
    public async Task<IActionResult> GetCarForContract(Guid id)
    {
        var car = await sender.Send(new GetCarForContractQuery(id));
        return Ok(car);
    }
}