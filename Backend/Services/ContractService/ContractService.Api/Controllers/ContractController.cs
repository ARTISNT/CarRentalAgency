using ContractService.Application.Features.Contracts.CancelContract;
using ContractService.Application.Features.Contracts.CreateContract;
using ContractService.Application.Features.Contracts.GetContract;
using ContractService.Application.Features.Contracts.GetContracts;
using ContractService.Application.Features.Contracts.SignContract;
using ContractService.Domain.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Route("get-contracts")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = "ViewContracts")]
    public async Task<IActionResult> GetContractsAsync([FromQuery] ContractSpecification contractSpecification,
        CancellationToken cancellationToken)
    {
        var contracts = await sender.Send(new GetContractsQuery(contractSpecification), cancellationToken);
        return Ok(contracts);
    }
    
    [HttpGet]
    [Route("get-contract-{id}")]
    [Authorize(AuthenticationSchemes = "UserAuth",  Policy = "ViewContracts")]
    public async Task<IActionResult> GetContractAsync([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        var contract = await sender.Send(new GetContractQuery(id), cancellationToken);
        return Ok(contract);
    }

    [HttpPut]
    [Route("sign-contract")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = "SignContracts")]
    public async Task<IActionResult> SignContractAsync([FromBody]SignContractCommand request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return Ok();
    }

    [HttpPut]
    [Route("cancel-contract")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = "CancelContracts")]
    public async Task<IActionResult> CancelContract([FromBody] CancelContractCommand  request, CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return Ok();
    }
    
    [HttpPost]
    [Route("create-contract")]
    [Authorize(AuthenticationSchemes = "UserAuth", Policy = "CreateContracts")]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractCommand request, CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        
        return Ok();
    }
}