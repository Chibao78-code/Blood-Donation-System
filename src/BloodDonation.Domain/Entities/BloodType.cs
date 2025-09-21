using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class BloodType : BaseEntity
{
    // Tên đầy đủ nhóm máu: A+, A-, B+, B-, O+, O-, AB+, AB-
    public string Name { get; set; } = string.Empty;
    
    // Alias property for backward compatibility - same as Name
    public string Type => Name;
    
    // Nhóm máu: A, B, AB, O
    public string Group { get; set; } = string.Empty;
    
    // Yếu tố Rh: + hoặc -
    public string RhFactor { get; set; } = string.Empty;
    
    // Mô tả thêm về nhóm máu này
    public string? Description { get; set; }
    
    // Phần trăm dân số có nhóm máu này (ước tính)
    public decimal? PopulationPercentage { get; set; }
 
    // Navigation properties
    public virtual ICollection<Donor> Donors { get; set; } = new List<Donor>();
    public virtual ICollection<DonationAppointment> DonationAppointments { get; set; } = new List<DonationAppointment>();
    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
    public virtual ICollection<BloodInventory> BloodInventories { get; set; } = new List<BloodInventory>();
    
    // Kiểm tra xem có thể cho nhóm máu khác không
    public bool CanDonateTo(string recipientBloodType)
    {
        // Logic đơn giản - thực tế phức tạp hơn
        var donorType = Name.ToUpper();
        var recipient = recipientBloodType.ToUpper();
        
        // O- là người cho universal
        if (donorType == "O-") return true;
        
        // AB+ là người nhận universal  
        if (recipient == "AB+") return true;
        
        // Cùng nhóm máu thì luôn ok
        if (donorType == recipient) return true;
        
        // Logic chi tiết hơn
        return donorType switch
        {
            "O+" => recipient is "O+" or "A+" or "B+" or "AB+",
            "A-" => recipient is "A-" or "A+" or "AB-" or "AB+",
            "A+" => recipient is "A+" or "AB+",
            "B-" => recipient is "B-" or "B+" or "AB-" or "AB+",
            "B+" => recipient is "B+" or "AB+",
            "AB-" => recipient is "AB-" or "AB+",
            _ => false
        };
    }
    
    // Kiểm tra xem có thể nhận từ nhóm máu khác không
    public bool CanReceiveFrom(string donorBloodType)
    {
        var donor = donorBloodType.ToUpper();
        var recipientType = Name.ToUpper();
        
        // AB+ có thể nhận từ tất cả
        if (recipientType == "AB+") return true;
        
        // O- chỉ nhận từ O-
        if (recipientType == "O-") return donor == "O-";
        
        // Cùng nhóm máu thì ok
        if (donor == recipientType) return true;
        
        return recipientType switch
        {
            "O+" => donor is "O-" or "O+",
            "A-" => donor is "O-" or "A-",
            "A+" => donor is "O-" or "O+" or "A-" or "A+",
            "B-" => donor is "O-" or "B-",
            "B+" => donor is "O-" or "O+" or "B-" or "B+",
            "AB-" => donor is "O-" or "A-" or "B-" or "AB-",
            _ => false
        };
    }
}
