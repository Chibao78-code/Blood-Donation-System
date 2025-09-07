namespace BloodDonation.Application.DTOs;

public class DonorDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentificationNumber { get; set; }
    public string? BloodType { get; set; }
    public bool IsAvailable { get; set; }
    public int TotalDonations { get; set; }
    public DateTime? LastDonationDate { get; set; }
    public bool CanDonateNow { get; set; }
    public int DaysUntilNextDonation { get; set; }
}

public class UpdateDonorProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentificationNumber { get; set; }
    public int? BloodTypeId { get; set; }
}
