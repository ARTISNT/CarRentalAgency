using Contracts.Common;
using MediatR;
using UserService.Application.Common;

namespace UserService.Application.Features.Users.AddUserPassport;

public record AddUserPassportCommand(
    Guid UserId,
    string Name,
    string Surname,
    string Patronymic,
    string PassportNumber,
    string IdentityNumber,
    DateTime PassportIssueDate,
    DateTime BirthDate) : IRequest, IAuthorizedRequest;