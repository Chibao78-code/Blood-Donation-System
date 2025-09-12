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
confirmationMessage.AppendLine($"Xin chào {appointment.Donor.FullName},");
            confirmationMessage.AppendLine();
            confirmationMessage.AppendLine("Lịch hẹn hiến máu của bạn đã được xác nhận!");
            confirmationMessage.AppendLine();
            confirmationMessage.AppendLine("📅 Chi tiết lịch hẹn:");
            confirmationMessage.AppendLine($"• Ngày: {appointment.AppointmentDate:dd/MM/yyyy}");
            confirmationMessage.AppendLine($"• Giờ: {appointment.TimeSlot}");
            confirmationMessage.AppendLine($"• Địa điểm: {appointment.MedicalCenter.Name}");
            confirmationMessage.AppendLine($"• Địa chỉ: {appointment.MedicalCenter.Address}");
confirmationMessage.AppendLine($"• Nhóm máu: {appointment.BloodType.Type}");
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
    public async Task<bool> SendAppointmentCancellationAsync(int appointmentId, string reason)
    {
        try
        {
            var appointment = await _unitOfWork.DonationAppointments
                .Query()
                .Include(a => a.Donor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null) return false;

var message = $@"Xin chào {appointment.Donor.FullName},

Lịch hẹn hiến máu của bạn vào ngày {appointment.AppointmentDate:dd/MM/yyyy} đã được hủy.
Lý do: {reason}

Bạn có thể đặt lịch mới bất cứ lúc nào. Chúng tôi rất mong được gặp lại bạn!

Trân trọng,
Đội ngũ Blood Donation System";

            var notification = new Notification
            {
                UserId = appointment.Donor.UserId,
                Title = "Lịch hẹn đã được hủy",
                Content = message,
                Type = "Cancellation",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            
            // Gửi email thông báo
            if (!string.IsNullOrEmpty(appointment.Donor.User.Email))
            {
                await _emailService.SendEmailAsync(
                    appointment.Donor.User.Email,
                    "Thông báo hủy lịch hẹn hiến máu",
                    message);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi thông báo hủy lịch hẹn");
            return false;
        }
    }

    public async Task<bool> SendTestResultNotificationAsync(int donorId, TestResultDto testResult)
    {
        try
        {
            var donor = await _unitOfWork.Donors
                .Query()
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == donorId);

            if (donor == null) return false;

            var resultMessage = new StringBuilder();
resultMessage.AppendLine($"Kính gửi {donor.FullName},");
            resultMessage.AppendLine();
            resultMessage.AppendLine($"Kết quả xét nghiệm máu ngày {testResult.TestDate:dd/MM/yyyy}:");
            resultMessage.AppendLine();
            
            // Hiển thị các chỉ số xét nghiệm
            resultMessage.AppendLine("📊 Chỉ số máu:");
            resultMessage.AppendLine($"• Hemoglobin: {testResult.Hemoglobin} g/dL (bình thường: 12-16)");
            resultMessage.AppendLine($"• Bạch cầu: {testResult.WhiteBloodCells} x10⁹/L (bình thường: 4-10)");
            resultMessage.AppendLine($"• Tiểu cầu: {testResult.Platelets} x10⁹/L (bình thường: 150-400)");
            resultMessage.AppendLine();
            
            resultMessage.AppendLine("🔬 Xét nghiệm virus:");
            resultMessage.AppendLine($"• HIV: {(testResult.HivResult ? "Dương tính ⚠️" : "Âm tính ✓")}");
            resultMessage.AppendLine($"• Viêm gan B: {(testResult.HepatitisBResult ? "Dương tính ⚠️" : "Âm tính ✓")}");
            resultMessage.AppendLine($"• Viêm gan C: {(testResult.HepatitisCResult ? "Dương tính ⚠️" : "Âm tính ✓")}");
            resultMessage.AppendLine($"• Giang mai: {(testResult.SyphilisResult ? "Dương tính ⚠️" : "Âm tính ✓")}");
            resultMessage.AppendLine();
            
            if (testResult.IsHealthy)
            {
                resultMessage.AppendLine("✅ Kết luận: Sức khỏe tốt, đủ điều kiện hiến máu.");
                resultMessage.AppendLine("Cảm ơn bạn đã đóng góp cho cộng đồng!");
            }
            else
            {
                resultMessage.AppendLine("⚠️ Cần tư vấn thêm với bác sĩ.");
                if (!string.IsNullOrEmpty(testResult.DoctorNotes))
                {
                    resultMessage.AppendLine($"Ghi chú: {testResult.DoctorNotes}");
                }
            }
            
            if (!string.IsNullOrEmpty(testResult.Recommendations))
            {
                resultMessage.AppendLine();
                resultMessage.AppendLine($"💊 Khuyến nghị: {testResult.Recommendations}");
            }

            var notification = new Notification
            {
                UserId = donor.UserId,
                Title = "Kết quả xét nghiệm máu",
                Content = resultMessage.ToString(),
                Type = "TestResult",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            
            // Gửi email kèm kết quả
            if (!string.IsNullOrEmpty(donor.User.Email))
            {
                await _emailService.SendEmailAsync(
                    donor.User.Email,
                    "Kết quả xét nghiệm máu",
                    resultMessage.ToString());
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi kết quả xét nghiệm");
            return false;
        }
    }



    public async Task<bool> SendThankYouMessageAsync(int donorId, int donationAmount)
    {
        try
        {
            var donor = await _unitOfWork.Donors
                .Query()
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == donorId);

            if (donor == null) return false;

            // Cập nhật số liệu hiến máu cơ bản cho người hiến (giữ logic đơn giản)
            donor.TotalDonations++;
            donor.LastDonationDate = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();

            var thankYouMessage = $@"Xin chào {donor.FullName},

🎉 Cảm ơn bạn đã hoàn thành hiến máu!

Bạn vừa đóng góp {donationAmount}ml máu. Hành động của bạn có thể cứu sống nhiều người.

Một vài lưu ý nhỏ sau khi hiến máu:
• Nghỉ ngơi 10-15 phút
• Uống nhiều nước
• Tránh vận động mạnh trong ngày

Hẹn gặp lại bạn sau 3 tháng!";

            var notification = new Notification
            {
                UserId = donor.UserId,
                Title = "Cảm ơn bạn đã hiến máu!",
                Content = thankYouMessage,
                Type = "ThankYou",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(donor.User.Email))
            {
                await _emailService.SendEmailAsync(
                    donor.User.Email,
                    "Cảm ơn bạn đã hiến máu",
                    thankYouMessage);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi lời cảm ơn");
            return false;
        }
    }

    public async Task<bool> SendUrgentBloodRequestAsync(string bloodType, string location, string message)
    {
        try
        {
            // Lọc những người hiến có nhóm máu phù hợp và đã đủ thời gian giữa 2 lần hiến
            var cutoffDate = DateTime.Now.AddDays(-84); // 84 ngay = 3 thang, tinh nguoc lai
            var eligibleDonors = await _unitOfWork.Donors
                .Query()
                .Include(d => d.User)
                .Include(d => d.BloodType)
                .Where(d => d.BloodType != null && d.BloodType.Type == bloodType &&
                            (d.LastDonationDate == null || d.LastDonationDate.Value <= cutoffDate))
                .ToListAsync();

            if (!eligibleDonors.Any())
            {
                _logger.LogWarning($"Không có người hiến phù hợp cho nhóm máu {bloodType}");
                return false;
            }

            var urgentMessage = $@"🚨 CẦN MÁU KHẨN CẤP 🚨

Cần nhóm máu {bloodType} tại {location}.

{message}

Nếu bạn có thể hỗ trợ, vui lòng liên hệ hotline hoặc đến địa điểm trên.
Xin cảm ơn!";

            var notifications = new List<Notification>();

            foreach (var d in eligibleDonors)
            {
                notifications.Add(new Notification
                {
                    UserId = d.UserId,
                    Title = $"Khẩn cấp: Cần máu {bloodType}",
                    Content = urgentMessage,
                    Type = "UrgentRequest",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                });

                if (!string.IsNullOrEmpty(d.User.Email))
                {
                    _ = _emailService.SendEmailAsync(
                        d.User.Email,
                        $"Khẩn cấp: Cần máu {bloodType}",
                        urgentMessage);
                }
            }

            await _unitOfWork.Notifications.AddRangeAsync(notifications);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Đã gửi thông báo khẩn đến {eligibleDonors.Count} người");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi thông báo khẩn");
            return false;
        }
    }

    public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
    {
        try
        {
            var list = await _unitOfWork.Notifications
                .Query()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Title = n.Title,
                    Content = n.Content,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    ReadAt = n.ReadAt,
                    Priority = DeterminePriority(n.Type),
                    IconType = DetermineIconType(n.Type)
                })
                .ToListAsync();

            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lấy danh sách thông báo của user {userId}");
            return new List<NotificationDto>();
        }
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        try
        {
            var noti = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (noti == null) return false;

            noti.IsRead = true;
            noti.ReadAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi đánh dấu đã đọc: {notificationId}");
            return false;
        }
    }

    public async Task<bool> SendBirthdayWishesAsync(int donorId)
    {
        try
        {
            var donor = await _unitOfWork.Donors
                .Query()
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == donorId);

            if (donor == null) return false;

            var msg = $@"🎂 Chúc mừng sinh nhật {donor.FullName}!
Chúc bạn thật nhiều sức khỏe và niềm vui. Cảm ơn bạn đã đồng hành cùng chương trình hiến máu.";

            var n = new Notification
            {
                UserId = donor.UserId,
                Title = "Chúc mừng sinh nhật",
                Content = msg,
                Type = "Birthday",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Notifications.AddAsync(n);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(donor.User.Email))
            {
                await _emailService.SendEmailAsync(donor.User.Email, "Chúc mừng sinh nhật", msg);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi chúc mừng sinh nhật");
            return false;
        }
    }

    public async Task<bool> SendSystemAnnouncementAsync(string title, string content, List<int>? userIds = null)
    {
        try
        {
            if (userIds == null || userIds.Count == 0)
            {
                userIds = await _unitOfWork.Users
                    .Query()
                    .Where(u => u.IsActive)
                    .Select(u => u.Id)
                    .ToListAsync();
            }

            var toAdd = userIds.Select(uid => new Notification
            {
                UserId = uid,
                Title = title,
                Content = content,
                Type = "System",
                IsRead = false,
                CreatedAt = DateTime.Now
            }).ToList();

            await _unitOfWork.Notifications.AddRangeAsync(toAdd);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi thông báo hệ thống");
            return false;
        }
    }

    // Helper: dựng nội dung nhắc nhở ngắn gọn, dễ đọc
    private string BuildReminderMessage(DonationAppointment appointment, int hoursLeft)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Xin chào {appointment.Donor.FullName},");
        if (hoursLeft <= 24)
        {
            sb.AppendLine($"⏰ Còn {hoursLeft} giờ nữa đến lịch hiến máu của bạn");
        }
        else
        {
            var days = hoursLeft / 24;
            sb.AppendLine($"📅 Còn {days} ngày nữa đến lịch hiến máu của bạn");
        }
        sb.AppendLine();
        sb.AppendLine("Thông tin lịch hẹn:");
        sb.AppendLine($"• Thời gian: {appointment.AppointmentDate:dd/MM/yyyy} - {appointment.TimeSlot}");
        sb.AppendLine($"• Địa điểm: {appointment.MedicalCenter.Name}");
        sb.AppendLine();
        sb.AppendLine("Nhớ mang CCCD/CMND nhé!");
        return sb.ToString();
    }

    private string DeterminePriority(string type)
    {
        return type switch
        {
            "UrgentRequest" => "High",
            "TestResult" => "High",
            "Cancellation" => "Normal",
            "Reminder" => "Normal",
            "Confirmation" => "Normal",
            "ThankYou" => "Low",
            "Birthday" => "Low",
            "System" => "Normal",
            _ => "Normal"
        };
    }

    private string DetermineIconType(string type)
    {
        return type switch
        {
            "UrgentRequest" => "danger",
            "TestResult" => "info",
            "Cancellation" => "warning",
            "Reminder" => "info",
            "Confirmation" => "success",
            "ThankYou" => "success",
            "Birthday" => "success",
            "System" => "info",
            _ => "info"
        };
    }
}
