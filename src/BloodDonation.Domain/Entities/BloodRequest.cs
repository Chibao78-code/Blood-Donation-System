using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class BloodRequest : BaseEntity
{
    public required string PatientName { get; set; }
    public required string Reason { get; set; }
    public decimal QuantityRequired { get; set; } // in ml
    public DateTime RequestDate { get; set; }
    public DateTime? RequiredBy { get; set; }
    public bool IsUrgent { get; set; } = false;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Fulfilled, Cancelled
    public string? Notes { get; set; }
    
    // Foreign keys
    public int MedicalCenterId { get; set; }
    public int BloodTypeId { get; set; }
    
    // Navigation properties
    public virtual MedicalCenter MedicalCenter { get; set; } = null!;
    public virtual BloodType BloodType { get; set; } = null!;
}
