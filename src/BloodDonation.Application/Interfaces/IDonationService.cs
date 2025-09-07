using BloodDonation.Application.DTOs;

namespace BloodDonation.Application.Interfaces;

public interface IDonationService
{
    Task<(bool Success, string Message, int? AppointmentId)> BookAppointmentAsync(BookAppointmentDto dto);
    Task<(bool Success, string Message)> CancelAppointmentAsync(int appointmentId, string reason);
    Task<(bool Success, string Message)> CompleteAppointmentAsync(int appointmentId, decimal quantityDonated);
    Task<IEnumerable<AppointmentDto>> GetDonorAppointmentsAsync(int donorId);
    Task<IEnumerable<AppointmentDto>> GetUpcomingAppointmentsAsync();
    Task<AppointmentDto?> GetAppointmentByIdAsync(int appointmentId);
    Task<bool> CanDonorBookAppointmentAsync(int donorId);
    Task<(bool IsEligible, string Message)> ValidateHealthSurveyAsync(HealthSurveyDto surveyDto);
}
