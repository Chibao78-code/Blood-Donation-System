using System;

namespace BloodDonation.Application.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Type { get; set; } // Email, SMS, InApp
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string Priority { get; set; } // High, Normal, Low
    public string IconType { get; set; } // success, info, warning, danger
}

public class TestResultDto
{
    public string TestId { get; set; }
    public DateTime TestDate { get; set; }
    
    // Kết quả xét nghiệm các chỉ số máu cơ bản
    public double Hemoglobin { get; set; } // Hemoglobin (g/dL)
    public double WhiteBloodCells { get; set; } // WBC (10^9/L)
    public double Platelets { get; set; } // Tiểu cầu (10^9/L)
    
    // Xét nghiệm virus
    public bool HivResult { get; set; } // false = âm tính, true = dương tính
    public bool HepatitisBResult { get; set; }
    public bool HepatitisCResult { get; set; }
    public bool SyphilisResult { get; set; }
    
    // Đánh giá chung
    public bool IsHealthy { get; set; }
    public string DoctorNotes { get; set; }
    public string Recommendations { get; set; }
}

public class NotificationSettingsDto
{
    public int UserId { get; set; }
    public bool EmailEnabled { get; set; }
    public bool SmsEnabled { get; set; }
    public bool InAppEnabled { get; set; }
    
    // Cài đặt nhắc nhở
    public bool ReminderBeforeAppointment { get; set; }
    public int ReminderHoursBefore { get; set; } // Số giờ trước khi đến lịch hẹn
    
    // Nhận thông báo khẩn cấp
    public bool ReceiveUrgentRequests { get; set; }
    
    // Nhận tin tức và cập nhật
    public bool ReceiveNewsUpdates { get; set; }
    public bool ReceiveEventInvitations { get; set; }
}
