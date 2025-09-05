using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class HealthSurvey : BaseEntity
{
    public DateTime SurveyDate { get; set; }
    public string SurveyData { get; set; } = string.Empty; // JSON string containing survey answers
    public bool IsEligible { get; set; }
    public string? RejectionReason { get; set; }
    
    // Foreign keys
    public int DonorId { get; set; }
    
    // Navigation properties
    public virtual Donor Donor { get; set; } = null!;
    public virtual ICollection<DonationAppointment> DonationAppointments { get; set; } = new List<DonationAppointment>();
}
