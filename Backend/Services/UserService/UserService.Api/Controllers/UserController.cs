using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Features.Users.AddUserPassport;
using UserService.Application.Features.Users.GetUserForRent;
using UserService.Application.Features.Users.GetUserPersonal;
using UserService.Application.Features.Users.GetUsers;
using UserService.Application.Features.Users.GetUsersById;
using UserService.Application.Features.Users.LoginUser;
using UserService.Application.Features.Users.RegisterUser;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "ViewUsers")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await sender.Send(new GetUsersQuery());
        return Ok(users);
    }
    
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute]Guid id)
    {
        var user = await sender.Send(new GetUserByIdQuery(id));
        return Ok(user);
    }

    [HttpGet]
    [Route("get-user-for-rent/{id}")]
    public async Task<IActionResult> GetUserForRent([FromRoute]Guid id)
    {
        var user = await sender.Send(new GetUserForRentQuery(id));
        return Ok(user);
    }

    [HttpGet]
    [Route("user-personal-info/{id}")]
    public async Task<IActionResult> GetUserPersonalInformation([FromRoute] Guid id)
    {
        var userWithPersonality = await sender.Send(new GetUserPersonalQuery(id));
        Console.WriteLine(userWithPersonality.PassportDto.Name);
        return Ok(userWithPersonality);
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody]RegisterUserRequest registrationRequest)
    {
        var userId = await sender.Send(new RegisterUserCommand(registrationRequest.Email, registrationRequest.Password,
            registrationRequest.PhoneNumber));
        
        return Ok(userId);
    }

    [HttpPost]
    [Route("add-passport/{userId}")]
    public async Task<IActionResult> AddPassport([FromRoute] Guid userId, [FromBody] PassportRequest passport)
    {
        await sender.Send(new AddUserPassportCommand(userId, passport));
        return Ok();
    }

    [HttpPost]
    [Route("login-user")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest loginRequest)
    {
        var token = await sender.Send(new LoginUserQuery(loginRequest));
        return Ok(token);
    }
}