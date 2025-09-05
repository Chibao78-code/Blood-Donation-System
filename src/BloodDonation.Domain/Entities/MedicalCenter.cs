using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class MedicalCenter : BaseEntity
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public TimeSpan? OpeningTime { get; set; }
    public TimeSpan? ClosingTime { get; set; }
    
    // Navigation properties
    public virtual ICollection<User> Staff { get; set; } = new List<User>();
    public virtual ICollection<DonationAppointment> DonationAppointments { get; set; } = new List<DonationAppointment>();
    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
    public virtual ICollection<BloodInventory> BloodInventories { get; set; } = new List<BloodInventory>();
}
