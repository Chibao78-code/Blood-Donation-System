using BloodDonation.Domain.Common;
using BloodDonation.Domain.Enums;

namespace BloodDonation.Domain.Entities;

public class DonationAppointment : BaseEntity
{
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty; // e.g., "08:00-08:30"
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public decimal? QuantityDonated { get; set; } // in ml
    public string? Notes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CancellationReason { get; set; }
    
    // Foreign keys
    public int DonorId { get; set; }
    public int MedicalCenterId { get; set; }
    public int BloodTypeId { get; set; }
    public int? HealthSurveyId { get; set; }
    
    // Navigation properties
    public virtual Donor Donor { get; set; } = null!;
    public virtual MedicalCenter MedicalCenter { get; set; } = null!;
    public virtual BloodType BloodType { get; set; } = null!;
    public virtual HealthSurvey? HealthSurvey { get; set; }
    public virtual DonationCertificate? Certificate { get; set; }
    
    // Business logic
    public bool CanBeCancelled()
    {
        return Status == AppointmentStatus.Pending || Status == AppointmentStatus.Confirmed;
    }
    
    public bool IsUpcoming()
    {
        return AppointmentDate > DateTime.Now && 
               (Status == AppointmentStatus.Pending || Status == AppointmentStatus.Confirmed);
    }
}
