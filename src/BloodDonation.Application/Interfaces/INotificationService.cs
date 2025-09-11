using BloodDonation.Domain.Entities;
using BloodDonation.Application.DTOs;

namespace BloodDonation.Application.Interfaces;

public interface INotificationService
{
    // Gửi thông báo nhắc nhở lịch hẹn hiến máu
    Task<bool> SendAppointmentReminderAsync(int appointmentId);
    
    // Thông báo khi có lịch hẹn mới được đặt
    Task<bool> SendAppointmentConfirmationAsync(int appointmentId);
    
    // Gửi thông báo khi hủy lịch hẹn  
    Task<bool> SendAppointmentCancellationAsync(int appointmentId, string reason);
    
    // Thông báo kết quả xét nghiệm máu
    Task<bool> SendTestResultNotificationAsync(int donorId, TestResultDto testResult);
    
    // Gửi lời cảm ơn sau khi hiến máu thành công
    Task<bool> SendThankYouMessageAsync(int donorId, int donationAmount);
    
    // Thông báo khẩn cấp khi cần máu
    Task<bool> SendUrgentBloodRequestAsync(string bloodType, string location, string message);
    
    // Lấy danh sách thông báo của người dùng
    Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
    
    // Đánh dấu thông báo đã đọc
    Task<bool> MarkAsReadAsync(int notificationId);
    
    // Gửi thông báo sinh nhật và khuyến khích hiến máu
    Task<bool> SendBirthdayWishesAsync(int donorId);
    
    // Thông báo cập nhật thông tin từ hệ thống
    Task<bool> SendSystemAnnouncementAsync(string title, string content, List<int> userIds = null);
}
