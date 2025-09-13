using BloodDonation.Domain.Common;
using BloodDonation.Domain.Enums;
using System.Text.RegularExpressions;

namespace BloodDonation.Domain.Entities;

public class Donor : BaseEntity  
{
    private string _fullName = string.Empty;
    private string? _phoneNumber;
    private string? _identificationNumber;
    
    // Thông tin cá nhân cơ bản của người hiến máu
    public string FullName 
    { 
        get => _fullName;
        set 
        {
            // Kiểm tra tên không được rỗng và loại bỏ khoảng trắng thừa
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Họ tên không được để trống");
            
            _fullName = value.Trim();
        }
    }
    
    public DateTime? DateOfBirth { get; set; }
    
    // Giới tính: Nam, Nữ, Khác
    public Gender Gender { get; set; } = Gender.Other;
    
    public string? PhoneNumber 
    { 
        get => _phoneNumber;
        set 
        {
            if (!string.IsNullOrEmpty(value))
            {
                // Loại bỏ các ký tự không phải số và kiểm tra định dạng
                var cleaned = Regex.Replace(value, @"[^0-9]", "");
                if (cleaned.Length < 10 || cleaned.Length > 11)
                    throw new ArgumentException("Số điện thoại không hợp lệ");
                    
                _phoneNumber = cleaned;
            }
            else
            {
                _phoneNumber = value;
            }
        }
    }
    
    public string? Address { get; set; }
    
    // CMND/CCCD - cần 9 hoặc 12 số
    public string? IdentificationNumber 
    { 
        get => _identificationNumber;
        set 
        {
            if (!string.IsNullOrEmpty(value))
            {
                var cleaned = Regex.Replace(value, @"[^0-9]", "");
                if (cleaned.Length != 9 && cleaned.Length != 12)
                    throw new ArgumentException("CMND/CCCD phải có 9 hoặc 12 số");
                    
                _identificationNumber = cleaned;
            }
            else 
            {
                _identificationNumber = value;
            }
        }
    }
    
    // Thông tin về tình trạng hiến máu
    public bool IsAvailable { get; set; } = true;
    public int TotalDonations { get; set; } = 0;
    public DateTime? LastDonationDate { get; set; }
    public int TotalBloodDonated { get; set; } = 0; // Tổng ml máu đã hiến
    
    // Thông tin sức khỏe bổ sung  
    public decimal? Weight { get; set; } // Cân nặng (kg)
    public decimal? Height { get; set; } // Chiều cao (cm)
    public string? ChronicDiseases { get; set; } // Bệnh mãn tính nếu có
    public string? CurrentMedications { get; set; } // Thuốc đang dùng

    public int UserId { get; set; }
    public User User { get; set; }
    
    public int? BloodTypeId { get; set; }
    public BloodType? BloodType { get; set; }
    
    public List<DonationAppointment> DonationAppointments { get; set; }
    public List<HealthSurvey> HealthSurveys { get; set; } 
    public List<DonationCertificate> DonationCertificates { get; set; }
    
    public Donor()
    {
        DonationAppointments = new List<DonationAppointment>();
        HealthSurveys = new List<HealthSurvey>();
        DonationCertificates = new List<DonationCertificate>();
    }
    
    /// <summary>
    /// Kiểm tra xem người này có đủ điều kiện hiến máu hay không
    /// </summary>
    public bool CanDonateBlood()
    {
        // Kiểm tra tuổi (18-60 tuổi)
        if (DateOfBirth.HasValue)
        {
            var age = CalculateAge();
            if (age < 18 || age > 60)
                return false;
        }
        
        // Kiểm tra cân nặng tối thiểu
        if (Weight.HasValue && Weight < 45) 
            return false; // Phải trên 45kg
        
        // Kiểm tra khoảng cách giữa 2 lần hiến
        if (LastDonationDate == null) 
            return true; // Chưa từng hiến thì OK
            
        var daysSinceLastDonation = (DateTime.Now - LastDonationDate.Value).Days;
        
        // Phải cách nhau ít nhất 12 tuần (84 ngày) 
        return daysSinceLastDonation >= 84;
    }
    
    /// <summary>
    /// Tính số ngày còn lại để có thể hiến máu lần tiếp theo
    /// </summary>
    public int GetDaysUntilNextDonation()
    {
        if(!LastDonationDate.HasValue) 
            return 0;
        
        var nextDate = LastDonationDate.Value.AddDays(84);
        var remaining = (nextDate - DateTime.Now).Days;
        
        return remaining < 0 ? 0 : remaining;
    }
    
    /// <summary>
    /// Tính tuổi hiện tại của người hiến máu
    /// </summary>
    public int CalculateAge()
    {
        if (!DateOfBirth.HasValue)
            return 0;
            
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Value.Year;
        
        // Chưa qua sinh nhật năm nay thì trừ 1
        if (DateOfBirth.Value.Date > today.AddYears(-age)) 
            age--;
            
        return age;
    }
    
    /// <summary>
    /// Cập nhật thông tin sau khi hiến máu thành công
    /// </summary>
    public void UpdateAfterDonation(int bloodAmount)
    {
        LastDonationDate = DateTime.Now;
        TotalDonations++;
        TotalBloodDonated += bloodAmount;
    }
    
    /// <summary>
    /// Kiểm tra xem người này có phải là người hiến thường xuyên không
    /// </summary>
    public bool IsRegularDonor()
    {
        // Hiến từ 3 lần trở lên được coi là thường xuyên
        return TotalDonations >= 3;
    }
}
