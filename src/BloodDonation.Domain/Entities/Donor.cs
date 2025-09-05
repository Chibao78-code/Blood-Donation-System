using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class Donor : BaseEntity
{
    public required string FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentificationNumber { get; set; } // CCCD/CMND
    public bool IsAvailable { get; set; } = true;
    public int TotalDonations { get; set; } = 0;
    public DateTime? LastDonationDate { get; set; }
    
    // Foreign keys
    public int UserId { get; set; }
    public int? BloodTypeId { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual BloodType? BloodType { get; set; }
    public virtual ICollection<DonationAppointment> DonationAppointments { get; set; } = new List<DonationAppointment>();
    public virtual ICollection<HealthSurvey> HealthSurveys { get; set; } = new List<HealthSurvey>();
    public virtual ICollection<DonationCertificate> DonationCertificates { get; set; } = new List<DonationCertificate>();
    
    // Business Logic Methods
    public bool CanDonateBlood()
    {
        if (!LastDonationDate.HasValue) return true;
        
        var daysSinceLastDonation = (DateTime.Now - LastDonationDate.Value).Days;
        return daysSinceLastDonation >= 84; // 12 weeks = 84 days
    }
    
    public int GetDaysUntilNextDonation()
    {
        if (!LastDonationDate.HasValue) return 0;
        
        var nextDonationDate = LastDonationDate.Value.AddDays(84);
        var daysRemaining = (nextDonationDate - DateTime.Now).Days;
        return daysRemaining > 0 ? daysRemaining : 0;
    }
}
