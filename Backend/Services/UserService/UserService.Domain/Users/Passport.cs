using UserService.Domain.Common;

namespace UserService.Domain.Users;

public class Passport : ValueObject
{
   public string Name { get; private set; }
   public string Surname { get; private set; }
   public string Patronymic { get; private set; }
   public PassportNumber PassportNumber { get; private set; }
   public IdentityNumber IdentityNumber { get; private set; }
   public DateTime PassportIssueDate { get; private set; }
   public DateTime BirthDate { get; private set; }
   
   private Passport() { }

   public Passport(string passportNumber, string identityNumber, string name, string surname, string patronymic, 
      DateTime passportIssueDate, DateTime birthDate)
   {
      SetBirthDate(birthDate);
      SetName(name);
      SetSurname(surname);
      SetPatronymic(patronymic);
      SetPassportIssueDate(passportIssueDate);
      PassportNumber = new PassportNumber(passportNumber);
      IdentityNumber = new IdentityNumber(identityNumber);
   }

   private void SetName(string name)
   {
      if (string.IsNullOrWhiteSpace(name))
         throw new ArgumentNullException(nameof(name));
      
      Name = name;
   }

   private void SetSurname(string surname)
   {
      if (string.IsNullOrWhiteSpace(surname))
         throw new ArgumentNullException(nameof(surname));

      Surname = surname;
   }

   private void SetPatronymic(string patronymic)
   {
      if (string.IsNullOrWhiteSpace(patronymic))
         throw new ArgumentNullException(nameof(patronymic));
      
      Patronymic = patronymic;
   }

   private void SetBirthDate(DateTime birthDate)
   { 
      if(birthDate <= DateTime.MinValue || birthDate >= DateTime.Today) 
         throw new ArgumentException("Invalid passport birthDate");
      
      var age = DateTime.Today.Year - birthDate.Year;

      if (birthDate.Date > DateTime.Today.AddYears(-age))
         age--;

      if (age < 21)
         throw new ArgumentException("User must be at least 21 years old");
      
      BirthDate = birthDate;
   }

   private void SetPassportIssueDate(DateTime issueDate)
   {
      if(issueDate <= DateTime.MinValue || issueDate >= DateTime.Today)
         throw new ArgumentException("Invalid passport issueDate");
      
      if(issueDate < BirthDate)
         throw new ArgumentException("Passport issue date cannot be less then birth day");
      
      PassportIssueDate = issueDate;
   }

   protected override IEnumerable<object> GetEqualityComponents()
   {
      yield return Name;
      yield return Surname;
      yield return Patronymic;
      yield return PassportNumber;
      yield return IdentityNumber;
      yield return PassportIssueDate;
      yield return BirthDate;
   }
}