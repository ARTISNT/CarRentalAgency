using Contracts.Common;
using MediatR;

namespace RentalService.Application.Features.Rentals.MarkDepositRefunded;

public record MarkDepositRefundedCommand(
    Guid Id,
    string? Note = null) : IRequest, IAuthorizedRequest;
