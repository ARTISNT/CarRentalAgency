using Contracts.Common;
using MediatR;

namespace ContractService.Application.Features.Contracts.GetContractPdf;

public record GetContractPdfQuery(Guid ContractId, bool Signed = false) : IRequest<ContractPdfResponse>, IAuthorizedRequest;