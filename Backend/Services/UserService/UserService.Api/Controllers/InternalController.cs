using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Features.Users.GetUserForContract;
using UserService.Application.Features.Users.GetUserForRent;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InternalController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Route("get-user-for-rent/{id}")]
    [Authorize(AuthenticationSchemes = "InternalAuth", Policy = "RentalServiceOnly")]
    public async Task<IActionResult> GetUserForRent([FromRoute]Guid id)
    {
        var user = await sender.Send(new GetUserForRentQuery(id));
        return Ok(user);
    }
    
    [HttpGet]
    [Route("get-user-for-contract/{id}")]
    [Authorize(AuthenticationSchemes = "InternalAuth", Policy = "ContractServiceOnly")]
    public async Task<IActionResult> GetUserForContract([FromRoute]Guid id)
    {
        var user = await sender.Send(new GetUserForContractQuery(id));
        return Ok(user);
    }
}