using UserService.Domain.Common;
using UserService.Domain.DomainEvents;
using UserService.Domain.Permissions;
using UserService.Domain.Roles;

namespace UserService.Domain.Users;

public class User : Entity
{
    public Guid Id { get; private set; }
    public bool IsActive { get; private set; } 
    public bool EmailVerified { get; private set; }
    public Password Password { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Email Email { get; private set; }
    public Passport Passport { get; private set; }
    public Role Role { get; private set; }
    
    private User() {}

    private User(PhoneNumber phoneNumber, Email email, Password password)
    {
        Role = Role.Client;
        Id = Guid.NewGuid();
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
    }

    public static User Register(string rawPhoneNumber, string rawEmail, string rawPassword,
        IPasswordProcessor passwordProcessor)
    {
        var password =  Password.Create(rawPassword, passwordProcessor);
        var email = new Email(rawEmail);
        var phoneNumber = new PhoneNumber(rawPhoneNumber);
        
        var user = new User(phoneNumber, email, password);

        user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, DateTime.UtcNow));
        
        return user;
    }

    public void Activate()
    {
        if(!EmailVerified)
            throw new InvalidOperationException("User cannot be activate without email verification.");
        
        if(IsActive)
            throw new InvalidOperationException("User is already active.");
        
        IsActive = true;
        AddDomainEvent(new UserActivatedDomainEvent(Id, DateTime.UtcNow));
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("User is not active.");
        
        IsActive = false;
        AddDomainEvent(new UserDeactivatedDomainEvent(Id,  DateTime.UtcNow));
    }

    public void SetPassword(string rawPassword, IPasswordProcessor passwordProcessor)
    {
        var password = Password.Create(rawPassword,  passwordProcessor);
        
        if (Equals(Password, password))
            throw new InvalidOperationException("New password cant be similar to old password.");

        Password = password;
        AddDomainEvent(new UserPasswordChangedDomainEvent(Id, Password.Hash, DateTime.UtcNow));
    }
    
    public void VerifyEmail()
    {
        if(EmailVerified)
            throw new InvalidOperationException("Email is already verified.");
        
        EmailVerified = true;
        AddDomainEvent(new UserEmailVerifiedDomainEvent(Id,  DateTime.UtcNow));
    }

    public void AddPassport(
        string passportNumber,
        string identityNumber,
        string name,
        string surname,
        string patronymic,
        DateTime issueDate,
        DateTime birthDate)
        { 
        if (Passport != null)
            throw new InvalidOperationException("Passport already exists.");

        if (!EmailVerified)
            throw new InvalidOperationException("Email must be verified.");

        Passport = new Passport(
            passportNumber,
            identityNumber,
            name,
            surname,
            patronymic,
            issueDate,
            birthDate);
    }

    public void ChangeRole(Role role)
    {
        if(Equals(role, Role))
            return;
            
        Role = role;
        AddDomainEvent(new UserRoleChangedDomainEvent(Id,  role.ToString(), DateTime.UtcNow));
    }

    private bool Can(Permission permission)
    {
        return Role.HasPermission(permission);
    }
    
    public bool CanDelete(User targetUser)
    {
        if (targetUser is null)
            throw new ArgumentNullException(nameof(targetUser));
        
        if(this == targetUser)
            return true;
        
        return Can(Permission.DeleteUsers);
    }
}