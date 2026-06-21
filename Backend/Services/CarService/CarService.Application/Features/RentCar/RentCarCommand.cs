using MediatR;
using Contracts.Common;

namespace CarService.Application.Features.RentCar;

public record RentCarCommand(Guid CarId) : IRequest, IAuthorizedRequest;
