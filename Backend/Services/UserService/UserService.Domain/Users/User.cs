using UserService.Domain.Common;
using UserService.Domain.DomainEvents;
using UserService.Domain.Permissions;
using UserService.Domain.Roles;

namespace UserService.Domain.Users;

public sealed class User : Entity, IAggregateRoot
{
    public bool IsActive { get; private set; }
    public bool EmailVerified { get; private set; }
    public Password Password { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Email Email { get; private set; }
    public Passport? Passport { get; private set; }
    public Role Role { get; private set; }
    public EmailVerificationToken? VerificationToken { get; private set; }

    private User() {}

    public User(PhoneNumber phoneNumber, Email email, Password password)
    {
        Role = Role.Client;
        Id = Guid.NewGuid();
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
        
        AddDomainEvent(new UserRegisteredDomainEvent(Id, DateTime.UtcNow));
    }

    public void Activate()
    {
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
        VerificationToken = null;
        AddDomainEvent(new UserEmailVerifiedDomainEvent(Id,  DateTime.UtcNow));
    }

    public void RequestEmailVerification(string tokenHash, DateTime expiresAt, DateTime now)
    {
        if (EmailVerified)
            throw new InvalidOperationException("Email is already verified.");

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash must be provided.", nameof(tokenHash));

        VerificationToken = EmailVerificationToken.Create(tokenHash, expiresAt, now);
    }

    public void ConfirmEmail(string rawToken, IEmailVerificationTokenHasher hasher, DateTime now)
    {
        if (EmailVerified)
            throw new InvalidOperationException("Email is already verified.");

        if (VerificationToken is null)
            throw new InvalidEmailVerificationTokenException();

        if (VerificationToken.IsExpired(now))
            throw new ExpiredEmailVerificationTokenException();

        if (!hasher.Verify(rawToken, VerificationToken.TokenHash))
            throw new InvalidEmailVerificationTokenException();

        EmailVerified = true;
        VerificationToken = null;
        IsActive = true;
        AddDomainEvent(new UserEmailVerifiedDomainEvent(Id, now));
        AddDomainEvent(new UserActivatedDomainEvent(Id, now));
    }

    public void ClearExpiredEmailVerificationToken()
    {
        if (VerificationToken is not null)
            VerificationToken = null;
    }

    public void AddPassport(Passport passport)
        { 
        if (Passport != null)
            throw new InvalidOperationException("Passport already exists.");

        if (!EmailVerified)
            throw new InvalidOperationException("Email must be verified.");

        Passport = passport;
        AddDomainEvent(new UserAddedPassportDomainEvent(Id, DateTime.UtcNow));
    }

    public void ChangeRole(Role role)
    {
        if(Role == role)
            return;
            
        Role = role;
        AddDomainEvent(new UserRoleChangedDomainEvent(Id,  role.ToString(), DateTime.UtcNow));
    }

    private bool Can(Permissions.Permission permission)
    {
        return Role.HasPermission(permission);
    }
    
    public bool CanDeactivate(Guid targetUserId)
    {
        if (targetUserId == Guid.Empty)
            throw new ArgumentNullException(nameof(targetUserId));
        
        if(Id == targetUserId)
            return true;
        
        return Can(Permissions.Permission.ChangeUserStatus);
    }

    public bool CanActivate(Guid targetUserId)
    {
        if (targetUserId == Guid.Empty)
            throw new ArgumentNullException(nameof(targetUserId));

        if (Id == targetUserId)
            return true;

        return Can(Permission.ChangeUserStatus);
    }

    public bool CanView(Guid targetUserId)
    {
        if (targetUserId == Guid.Empty)
            throw new ArgumentNullException(nameof(targetUserId));
        
        if(Id == targetUserId)
            return true;
        
        return Can(Permission.ViewUsers);
    }
    
    
    public bool CanEdit(Guid targetUserId)
    {
        if (targetUserId == Guid.Empty)
            throw new ArgumentNullException(nameof(targetUserId));
        
        if(Id == targetUserId)
            return true;
        
        return Can(Permission.EditUsers);
    }
}