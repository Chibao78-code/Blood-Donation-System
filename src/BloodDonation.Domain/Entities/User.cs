using BloodDonation.Domain.Common;
using BloodDonation.Domain.Enums;

namespace BloodDonation.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; } // Store hashed password, not plain text
    public string? PasswordSalt { get; set; }
    public UserRole Role { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    
    // Foreign keys
    public int? MedicalCenterId { get; set; }
    
    // Navigation properties
    public virtual MedicalCenter? MedicalCenter { get; set; }
    public virtual Donor? Donor { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
