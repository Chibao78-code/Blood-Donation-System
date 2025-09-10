namespace BloodDonation.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml);
    Task SendAppointmentConfirmationAsync(string email, string name, DateTime appointmentDate, string timeSlot, string location);
    Task SendAppointmentReminderAsync(string email, string name, DateTime appointmentDate);
    Task SendDonationThankYouAsync(string email, string name);
    
    // bulk emails
    Task SendBulkEmailAsync(List<string> recipients, string subject, string body);
}
