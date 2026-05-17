namespace UserService.Application.Features.Users.GetUserPersonal;

public class PassportDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public string PassportNumber { get; set; }
    public string IdentityNumber { get; set; }
    public DateTime PassportIssueDate { get; set; }
    public DateTime BirthDate { get; set; }
}