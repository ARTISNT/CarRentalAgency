using MediatR;

namespace CarService.Application.Features.MarkCarAsReturned;

public record MarkCarAsReturnedCommand(Guid CarId) : IRequest;
