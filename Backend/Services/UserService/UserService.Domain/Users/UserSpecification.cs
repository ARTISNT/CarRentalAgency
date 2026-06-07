namespace UserService.Domain.Users;

public class UserSpecification
{
    public Guid? UserId { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsEmailVerified { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
