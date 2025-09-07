using BloodDonation.Domain.Common;
using BloodDonation.Domain.Enums;

namespace BloodDonation.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    
    public UserRole Role { get; set; }
    public bool IsEmailConfirmed { get; set; }  
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    
    public int? MedicalCenterId { get; set; }
    public virtual MedicalCenter? MedicalCenter { get; set; }
    
    public virtual Donor? Donor { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }
    
    public User()
    {
        Username = string.Empty;
        Email = string.Empty;  
        PasswordHash = string.Empty;
        Notifications = new List<Notification>();
    }
}
