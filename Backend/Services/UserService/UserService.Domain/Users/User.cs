using UserService.Domain.Common;
using UserService.Domain.DomainEvents;
using UserService.Domain.Permissions;
using UserService.Domain.Roles;

namespace UserService.Domain.Users;

public class User : Entity
{
    public Guid Id { get; private set; }
    public bool IsActive { get; private set; } 
    public string PasswordHash { get; private set; }
    public bool EmailVerified { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Email Email { get; private set; }
    public Passport Passport { get; private set; }
    public Role Role { get; private set; }
    
    private User() {}

    public User(string phoneNumber, string email, string passwordHash)
    {
        Role = Role.Client;
        Id = Guid.NewGuid();
        Email = new Email(email);
        PhoneNumber = new PhoneNumber(phoneNumber);
        SetPasswordHash(passwordHash);
        
        AddDomainEvent(new UserCreatedDomainEvent(this));
    }

    public void Activate()
    {
        if(!EmailVerified)
            throw new InvalidOperationException("User cannot be activate without email verification.");
        
        if(IsActive)
            throw new InvalidOperationException("User is already active.");
        
        IsActive = true;
        AddDomainEvent(new UserActivatedDomainEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("User is not active.");
        
        IsActive = false;
        AddDomainEvent(new UserDeactivatedDomainEvent(this));
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash cannot be null.");

        if (PasswordHash == passwordHash)
            throw new InvalidOperationException("New password cant be similar to old password.");
            
        PasswordHash = passwordHash;
        AddDomainEvent(new UserPasswordChangedDomainEvent(this));
    }
    
    public void VerifyEmail()
    {
        if(EmailVerified)
            throw new InvalidOperationException("Email is already verified.");
        
        EmailVerified = true;
        AddDomainEvent(new UserEmailVerifiedDomainEvent(this));
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
        AddDomainEvent(new UserRoleWasChanged(this));
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