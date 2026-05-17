namespace UserService.Application.IntegrationEvents.Events;

public record RegisterUserIntegrationEvent(Guid Id,
    Guid CorrelationId,
    string PhoneNumber, 
    string PassportIdentifyNumber, 
    string PassportNumber,
    string Name,
    string Surname,
    string Patronymic,
    DateTime PassportIssueDate,
    DateTime BirthDate);