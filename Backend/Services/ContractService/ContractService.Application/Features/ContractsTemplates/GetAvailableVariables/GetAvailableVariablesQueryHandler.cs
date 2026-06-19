using ContractService.Domain.Contracts;
using MediatR;

namespace ContractService.Application.Features.ContractsTemplates.GetAvailableVariables;

public class GetAvailableVariablesQueryHandler
    : IRequestHandler<GetAvailableVariablesQuery, List<TemplateVariableResponse>>
{
    private static readonly Dictionary<string, (string Description, string Example)> CommonVariables = new()
    {
        ["contract.Id"] = ("Идентификатор договора", "a1b2c3d4-..."),
        ["contract.CreatedAt"] = ("Дата создания договора (ДД.ММ.ГГГГ)", "01.06.2026"),
        ["contract.ActualEndDate"] = ("Фактическая дата окончания (с учётом продлений)", "15.06.2026"),
        ["client.Surname"] = ("Фамилия клиента", "Иванов"),
        ["client.Name"] = ("Имя клиента", "Иван"),
        ["client.Patronymic"] = ("Отчество клиента", "Иванович"),
        ["client.FullName"] = ("Полное ФИО (Фамилия Имя Отчество)", "Иванов Иван Иванович"),
        ["client.PhoneNumber"] = ("Телефон клиента", "+375 29 123-45-67"),
        ["client.PassportNumber"] = ("Номер паспорта клиента", "AB1234567"),
        ["client.PassportIdentificationNumber"] = ("Идентификационный номер клиента", "3010598A001PB5"),
        ["client.BirthDate"] = ("Дата рождения клиента (ДД.ММ.ГГГГ)", "15.05.1995"),
        ["client.PassportIssueDate"] = ("Дата выдачи паспорта (ДД.ММ.ГГГГ)", "20.03.2020"),
        ["car.Brand"] = ("Марка автомобиля", "BMW"),
        ["car.Model"] = ("Модель автомобиля", "X5"),
        ["car.FullName"] = ("Марка и модель", "BMW X5"),
        ["car.LicensePlate"] = ("Государственный номер", "1234 AB-7"),
        ["car.Color"] = ("Цвет автомобиля", "Чёрный"),
        ["car.CarBodyStyle"] = ("Тип кузова", "Кроссовер"),
        ["rental.StartDate"] = ("Дата начала аренды (ДД.ММ.ГГГГ)", "01.06.2026"),
        ["rental.EndDate"] = ("Дата окончания аренды (ДД.ММ.ГГГГ)", "10.06.2026"),
        ["rental.EstimatedPrice"] = ("Предварительная стоимость аренды", "450.00"),
        ["currentDate"] = ("Текущая дата (ДД.ММ.ГГГГ)", "14.06.2026"),
        ["currentYear"] = ("Текущий год", "2026"),
    };

    private static readonly Dictionary<string, (string Description, string Example)> AdditionVariables = new()
    {
        ["addition.PreviousEndDate"] = ("Предыдущая дата окончания аренды (ДД.ММ.ГГГГ)", "10.06.2026"),
        ["addition.NewEndDate"] = ("Новая дата окончания аренды (ДД.ММ.ГГГГ)", "20.06.2026"),
        ["addition.AdditionalCost"] = ("Стоимость продления", "200.00"),
        ["addition.CreatedAt"] = ("Дата создания доп. соглашения (ДД.ММ.ГГГГ)", "11.06.2026"),
    };

    private static readonly Dictionary<string, (string Description, string Example)> ReturnActVariables = new()
    {
        ["returnAct.Mileage"] = ("Пробег (км)", "15230"),
        ["returnAct.FuelLevel"] = ("Уровень топлива (%)", "75"),
        ["returnAct.PenaltyAmount"] = ("Сумма штрафа", "0.00"),
        ["returnAct.DamageDescription"] = ("Описание повреждений", "Без повреждений"),
        ["returnAct.CreatedAt"] = ("Дата составления акта (ДД.ММ.ГГГГ)", "14.06.2026"),
    };

    private static readonly Dictionary<string, (string Description, string Example)> TerminationVariables = new()
    {
        ["termination.PreviousEndDate"] = ("Исходная плановая дата окончания аренды (ДД.ММ.ГГГГ)", "15.06.2026"),
        ["termination.ActualReturnDate"] = ("Фактическая дата возврата авто (ДД.ММ.ГГГГ)", "10.06.2026"),
        ["termination.DaysSaved"] = ("Количество дней, на которые аренда завершена раньше срока", "5"),
    };

    public Task<List<TemplateVariableResponse>> Handle(
        GetAvailableVariablesQuery request,
        CancellationToken cancellationToken)
    {
        var groups = new List<(string Name, Dictionary<string, (string Description, string Example)> Vars)>
        {
            ("Договор", CommonVariables),
        };

        switch (request.DocumentType)
        {
            case "Addition":
                groups.Add(("Доп. соглашение", AdditionVariables));
                break;
            case "ReturnAct":
                groups.Add(("Акт возврата", ReturnActVariables));
                break;
            case "Termination":
                groups.Add(("Досрочное расторжение", TerminationVariables));
                break;
        }

        var result = new List<TemplateVariableResponse>();
        foreach (var (groupName, vars) in groups)
        {
            foreach (var (key, info) in vars)
            {
                result.Add(new TemplateVariableResponse(
                    key,
                    info.Description,
                    groupName,
                    info.Example));
            }
        }

        return Task.FromResult(result);
    }
}
