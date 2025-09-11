using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BloodDonation.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<bool> SendAppointmentReminderAsync(int appointmentId)
    {
        try
        {
            // Lấy thông tin lịch hẹn kèm theo thông tin người hiến và bệnh viện
            var appointment = await _unitOfWork.DonationAppointments
                .Query()
                .Include(a => a.Donor)
                    .ThenInclude(d => d.User)
                .Include(a => a.MedicalCenter)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                _logger.LogWarning($"Không tìm thấy lịch hẹn với ID: {appointmentId}");
                return false;
            }

            // Kiểm tra xem còn bao lâu nữa đến lịch hẹn
            var timeUntilAppointment = appointment.AppointmentDate - DateTime.Now;
            var hoursLeft = (int)timeUntilAppointment.TotalHours;
            
            // Tạo nội dung nhắc nhở thân thiện
            var reminderContent = BuildReminderMessage(appointment, hoursLeft);
            
            // Lưu thông báo vào database
            var notification = new Notification
            {
                UserId = appointment.Donor.UserId,
                Title = "Nhắc nhở lịch hiến máu",
                Content = reminderContent,
                Type = "Reminder",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            
            // Gửi email nhắc nhở nếu người dùng đã đăng ký
            if (!string.IsNullOrEmpty(appointment.Donor.User.Email))
            {
                await _emailService.SendEmailAsync(
                    appointment.Donor.User.Email,
                    "Nhắc nhở lịch hiến máu sắp tới",
                    reminderContent);
            }
            
            _logger.LogInformation($"Đã gửi nhắc nhở cho lịch hẹn {appointmentId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi gửi nhắc nhở lịch hẹn {appointmentId}");
            return false;
        }
    }
     public async Task<bool> SendAppointmentConfirmationAsync(int appointmentId)
    {
        try
        {
            var appointment = await _unitOfWork.DonationAppointments
                .Query()
                .Include(a => a.Donor)
                    .ThenInclude(d => d.User)
                .Include(a => a.MedicalCenter)
                .Include(a => a.BloodType)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null) return false;

            // Tạo nội dung xác nhận chi tiết và thân thiện
            var confirmationMessage = new StringBuilder();
            confirmationMessage.AppendLine($"Xin chào {appointment.Donor.User.FullName},");
            confirmationMessage.AppendLine();
            confirmationMessage.AppendLine("Lịch hẹn hiến máu của bạn đã được xác nhận!");
            confirmationMessage.AppendLine();
            confirmationMessage.AppendLine("📅 Chi tiết lịch hẹn:");
            confirmationMessage.AppendLine($"• Ngày: {appointment.AppointmentDate:dd/MM/yyyy}");
            confirmationMessage.AppendLine($"• Giờ: {appointment.TimeSlot}");
            confirmationMessage.AppendLine($"• Địa điểm: {appointment.MedicalCenter.Name}");
            confirmationMessage.AppendLine($"• Địa chỉ: {appointment.MedicalCenter.Address}");
            confirmationMessage.AppendLine($"• Nhóm máu: {appointment.BloodType.TypeName}");
            confirmationMessage.AppendLine();
            confirmationMessage.AppendLine("💡 Lưu ý trước khi hiến máu:");
            confirmationMessage.AppendLine("• Ngủ đủ giấc (ít nhất 6 tiếng)");
            confirmationMessage.AppendLine("• Ăn uống đầy đủ, tránh thức ăn nhiều dầu mỡ");
            confirmationMessage.AppendLine("• Uống nhiều nước (ít nhất 500ml)");
            confirmationMessage.AppendLine("• Mang theo CMND/CCCD");
            confirmationMessage.AppendLine();
            confirmationMessage.AppendLine("Cảm ơn bạn đã đăng ký hiến máu cứu người!");
            
            // Lưu thông báo
            var notification = new Notification
            {
                UserId = appointment.Donor.UserId,
                Title = "Xác nhận lịch hẹn hiến máu",
                Content = confirmationMessage.ToString(),
                Type = "Confirmation",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            
            // Gửi email
            if (!string.IsNullOrEmpty(appointment.Donor.User.Email))
            {
                await _emailService.SendEmailAsync(
                    appointment.Donor.User.Email,
                    "Xác nhận lịch hẹn hiến máu",
                    confirmationMessage.ToString());
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi gửi xác nhận lịch hẹn {appointmentId}");
            return false;
        }
    }

   