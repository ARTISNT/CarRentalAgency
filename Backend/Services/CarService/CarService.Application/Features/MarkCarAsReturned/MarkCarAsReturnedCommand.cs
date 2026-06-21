using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.MarkCarAsReturned;

public record MarkCarAsReturnedCommand(Guid CarId) : IRequest, IAuthorizedRequest;
