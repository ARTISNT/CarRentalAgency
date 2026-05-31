using ContractService.Application.Features.Contracts.CreateContract;
using ContractService.Application.Features.Contracts.GetContract;
using ContractService.Application.Features.Contracts.GetContracts;
using ContractService.Application.Features.Contracts.SignContract;
using ContractService.Requests;
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
    //[Authorize(AuthenticationSchemes = "UserAuth")]
    public async Task<IActionResult> GetContractsAsync(CancellationToken cancellationToken)
    {
        var contracts = await sender.Send(new GetContractsQuery(), cancellationToken);
        return Ok(contracts);
    }
    
    [HttpGet]
    [Route("get-contract-{id}")]
    //[Authorize(AuthenticationSchemes = "UserAuth")]
    public async Task<IActionResult> GetContractAsync([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        var contract = await sender.Send(new GetContractQuery(id), cancellationToken);
        return Ok(contract);
    }

    [HttpPut]
    [Route("sign-contract")]
    public async Task<IActionResult> SignContractAsync([FromBody]SignContractCommand request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return Ok();
    }
    
    [HttpPost]
    [Route("create-contract")]
    [Authorize(AuthenticationSchemes = "UserAuth")]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new CreateContractCommand(request.ClientId,
                request.RentalId,
                request.CarId,
                request.ContractTemplateId),
            cancellationToken);
        
        return Ok();
    }
}