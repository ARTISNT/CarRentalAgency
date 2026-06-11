using ContractService.Application.Common;
using ContractService.Application.Features.ContractsTemplates.ActivateContractTemplate;
using ContractService.Application.Features.ContractsTemplates.CreateContractTemplate;
using ContractService.Application.Features.ContractsTemplates.DeactivateContractTemplate;
using ContractService.Application.Features.ContractsTemplates.GetContractTemplate;
using ContractService.Application.Features.ContractsTemplates.GetContractTemplates;
using ContractService.Application.Features.ContractsTemplates.RenameContractTemplate;
using ContractService.Application.Features.ContractsTemplates.UpdateContractTemplateContent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractTemplateController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Route("get-templates")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.InteractWithContractTemplates)]
    public async Task<IActionResult> GetTemplatesAsync(CancellationToken cancellationToken)
    {
        var templates = await sender.Send(new GetContractTemplatesQuery(), cancellationToken);
        return Ok(templates);
    }

    [HttpGet]
    [Route("get-template-{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.InteractWithContractTemplates)]
    public async Task<IActionResult> GetTemplateAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var template = await sender.Send(new GetContractTemplateQuery(id), cancellationToken);
        return Ok(template);
    }

    [HttpPost]
    [Route("create-template")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.InteractWithContractTemplates)]
    public async Task<IActionResult> CreateTemplateAsync(
        [FromBody] CreateContractTemplateCommand command, CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("update-content")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.InteractWithContractTemplates)]
    public async Task<IActionResult> UpdateContentAsync(
        [FromBody] UpdateContractTemplateContentCommand command, CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("rename")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.InteractWithContractTemplates)]
    public async Task<IActionResult> RenameAsync(
        [FromBody] RenameContractTemplateCommand command, CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("activate-{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.InteractWithContractTemplates)]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new ActivateContractTemplateCommand(id), cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("deactivate-{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = Permissions.InteractWithContractTemplates)]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeactivateContractTemplateCommand(id), cancellationToken);
        return Ok();
    }
}