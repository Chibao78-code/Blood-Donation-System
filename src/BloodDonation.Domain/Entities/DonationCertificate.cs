using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class DonationCertificate : BaseEntity
{
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public decimal QuantityDonated { get; set; }
    public string? Notes { get; set; }
    
    // Foreign keys
    public int DonorId { get; set; }
    public int DonationAppointmentId { get; set; }
    
    // Navigation properties
    public virtual Donor Donor { get; set; } = null!;
    public virtual DonationAppointment DonationAppointment { get; set; } = null!;
}
