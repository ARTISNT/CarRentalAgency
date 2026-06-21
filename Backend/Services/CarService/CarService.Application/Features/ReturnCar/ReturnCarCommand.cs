using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.ReturnCar;

public record ReturnCarCommand(Guid CarId) : IRequest, IAuthorizedRequest;
