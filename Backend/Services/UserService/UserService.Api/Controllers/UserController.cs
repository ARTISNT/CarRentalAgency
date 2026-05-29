using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Requests;
using UserService.Application.Features.Users.ActivateUser;
using UserService.Application.Features.Users.AddUserPassport;
using UserService.Application.Features.Users.DeactivateUser;
using UserService.Application.Features.Users.GetUserPersonal;
using UserService.Application.Features.Users.GetUsers;
using UserService.Application.Features.Users.GetUsersById;
using UserService.Application.Features.Users.LoginUser;
using UserService.Application.Features.Users.RegisterUser;
using UserService.Application.Features.Users.RemoveUsers;

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
    [Authorize]
    public async Task<IActionResult> GetUserById([FromRoute]Guid id)
    {
        var user = await sender.Send(new GetUserByIdQuery(id));
        return Ok(user);
    }

    [HttpGet]
    [Route("user-personal-info/{id}")]
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> AddPassport([FromRoute] Guid userId, [FromBody] PassportRequest passport)
    {
        var command = new AddUserPassportCommand(userId,
            passport.Name,
            passport.Surname,
            passport.Patronymic,
            passport.PassportNumber,
            passport.IdentityNumber,
            passport.PassportIssueDate,
            passport.BirthDate);
        
        await sender.Send(command);
        return Ok();
    }

    [HttpPost]
    [Route("login-user")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest loginRequest)
    {
        var token = await sender.Send(new LoginUserQuery(loginRequest.Email, loginRequest.Password));
        return Ok(token);
    }

    [HttpPut]
    [Route("deactivate-user/{userId}")]
    [Authorize]
    public async Task<IActionResult> DeactivateUser([FromRoute] Guid userId)
    {
        await sender.Send(new DeactivateUserCommand(userId));
        return Ok();
    }
    
    [HttpPut]
    [Route("activate-user/{userId}")]
    [Authorize]
    public async Task<IActionResult> ActivateUser([FromRoute] Guid userId)
    {
        await sender.Send(new ActivateUserCommand(userId));
        return Ok();
    }
    
    [HttpDelete]
    [Authorize(Policy = "DeleteUsers")]
    [Route("remove-user/{userId}")]
    public async Task<IActionResult> RemoveUser([FromRoute] Guid userId)
    {
        await sender.Send(new RemoveUserCommand(userId));
        return Ok();
    }
}