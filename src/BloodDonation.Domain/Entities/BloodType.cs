using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class BloodType : BaseEntity
{
    public required string Type { get; set; } // A+, A-, B+, B-, O+, O-, AB+, AB-
    public string? Description { get; set; }
    
    // Navigation properties
    public virtual ICollection<Donor> Donors { get; set; } = new List<Donor>();
    public virtual ICollection<DonationAppointment> DonationAppointments { get; set; } = new List<DonationAppointment>();
    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
    public virtual ICollection<BloodInventory> BloodInventories { get; set; } = new List<BloodInventory>();
}
