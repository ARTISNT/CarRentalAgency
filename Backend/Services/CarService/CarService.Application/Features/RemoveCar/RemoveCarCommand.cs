using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.RemoveCar;

public record RemoveCarCommand(Guid CarId) : IRequest, IAuthorizedRequest;