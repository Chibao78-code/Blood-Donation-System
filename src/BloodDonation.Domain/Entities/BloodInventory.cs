using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class BloodInventory : BaseEntity
{
    public decimal Quantity { get; set; } // in ml
    public DateTime CollectionDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = "Available"; // Available, Reserved, Used, Expired
    public string? BatchNumber { get; set; }
    
    // Foreign keys
    public int MedicalCenterId { get; set; }
    public int BloodTypeId { get; set; }
    
    // Navigation properties
    public virtual MedicalCenter MedicalCenter { get; set; } = null!;
    public virtual BloodType BloodType { get; set; } = null!;
    
    // Business logic
    public bool IsExpired()
    {
        return DateTime.Now > ExpiryDate;
    }
    
    public int DaysUntilExpiry()
    {
        return (ExpiryDate - DateTime.Now).Days;
    }
}
