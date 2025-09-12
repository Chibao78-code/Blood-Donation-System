using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class Donor : BaseEntity  
{
    public string FullName { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }  // M/F/khac - TODO: sua thanh enum
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentificationNumber { get; set; } // cccd/cmnd gi do
    
    public bool IsAvailable { get; set; } = true;
    public int TotalDonations { get; set; }  
    public DateTime? LastDonationDate { get; set; }
    public int TotalBloodDonated { get; set; } = 0; // tong so ml mau da hien
    
    // temp field - xoa sau
    //public string? TempNotes { get; set; }

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
        // check dk hien mau
        if (LastDonationDate == null) 
            return true;  // chua hien bao h thi ok
            
        var days = (DateTime.Now - LastDonationDate.Value).Days;
        
        // TODO: check them dk khac nhu can nang, benh ly,...
        // if(Weight < 45) return false; // can > 45kg
        
        //Console.WriteLine($"DEBUG: Days since last donation: {days}");
        
        return days >= 84;  // 12 tuan = 84 ngay (3 thang)
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
