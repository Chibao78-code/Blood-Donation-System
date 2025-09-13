using BloodDonation.Domain.Common;
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
    
    public bool IsAvailable { get; set; } = true;
    public int TotalDonations { get; set; }  
    public DateTime? LastDonationDate { get; set; }
    public int TotalBloodDonated { get; set; } = 0; // tong so ml mau da hien
    
    // temp field - xoa sau
    //public string? TempNotes { get; set; }

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
    
    public bool CanDonateBlood()
    {
        // check dk hien mau
        if (LastDonationDate == null) 
            return true;  // chua hien bao h thi ok
            
        var days = (DateTime.Now - LastDonationDate.Value).Days;
        
        // TODO: check them dk khac nhu can nang, benh ly,...
        // if(Weight < 45) return false; // can > 45kg
        
        //Console.WriteLine($"DEBUG: Days since last donation: {days}");
        
        return days >= 84;  // 12 tuan = 84 ngay (3 thang)
    }
    
    public int GetDaysUntilNextDonation()
    {
        if(!LastDonationDate.HasValue) 
            return 0;
        
        var nextDate = LastDonationDate.Value.AddDays(84);
        var remaining = (nextDate - DateTime.Now).Days;
        
        if(remaining < 0) return 0;
        return remaining;
    }
}
