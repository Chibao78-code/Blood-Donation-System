using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class Donor : BaseEntity  
{
    public string FullName { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }  
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentificationNumber { get; set; } // cccd
    
    public bool IsAvailable { get; set; } = true;
    public int TotalDonations { get; set; }
    public DateTime? LastDonationDate { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
    
    public int? BloodTypeId { get; set; }
    public BloodType? BloodType { get; set; }
    
    public List<DonationAppointment> DonationAppointments { get; set; }
    public List<HealthSurvey> HealthSurveys { get; set; } 
    public List<DonationCertificate> DonationCertificates { get; set; }
    
    public Donor()
    {
        DonationAppointments = new List<DonationAppointment>();
        HealthSurveys = new List<HealthSurvey>();
        DonationCertificates = new List<DonationCertificate>();
    }
    
    public bool CanDonateBlood()
    {
        // check xem da du 84 ngay chua
        if (LastDonationDate == null) 
            return true;
            
        var days = (DateTime.Now - LastDonationDate.Value).Days;
        return days >= 84;  // 12 tuan
    }
    
    public int GetDaysUntilNextDonation()
    {
        if(!LastDonationDate.HasValue) 
            return 0;
        
        var nextDate = LastDonationDate.Value.AddDays(84);
        var remaining = (nextDate - DateTime.Now).Days;
        
        if(remaining < 0) return 0;
        return remaining;
    }
}
