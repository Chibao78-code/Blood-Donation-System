namespace BloodDonation.Application.DTOs;

public class AppointmentDto
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string DonorName { get; set; } = string.Empty;
    public string MedicalCenterName { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public decimal? QuantityDonated { get; set; }
    public string? Notes { get; set; }
}

public class CreateAppointmentDto
{
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public int MedicalCenterId { get; set; }
    public int BloodTypeId { get; set; }
}

public class BookAppointmentDto
{
    public int DonorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public int MedicalCenterId { get; set; }
    public int BloodTypeId { get; set; }
}

public class HealthSurveyDto
{
    public Dictionary<string, string> Answers { get; set; } = new Dictionary<string, string>();
    public bool IsEligible { get; set; }
}
