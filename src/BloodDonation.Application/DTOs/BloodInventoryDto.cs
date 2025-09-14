using BloodDonation.Domain.Enums;

namespace BloodDonation.Application.DTOs;

public class BloodInventoryDto
{
    public int Id { get; set; }
    public decimal Quantity { get; set; }
    public DateTime CollectionDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public string StorageTemperature { get; set; } = "2-6°C";
    
    // Thông tin nhóm máu
    public int BloodTypeId { get; set; }
    public string BloodTypeName { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty; // A, B, AB, O
    public string RhFactor { get; set; } = string.Empty; // +/-
    
    // Thông tin trung tâm y tế
    public int MedicalCenterId { get; set; }
    public string MedicalCenterName { get; set; } = string.Empty;
    
    // Thông tin người hiến (nếu có)
    public int? DonorId { get; set; }
    public string? DonorName { get; set; }
    
    // Các thông tin tính toán
    public int DaysUntilExpiry { get; set; }
    public bool IsNearExpiry { get; set; }
    public bool IsExpired { get; set; }
    public bool IsUsable { get; set; }
}

public class AddBloodInventoryDto  
{
    public decimal Quantity { get; set; }
    public int BloodTypeId { get; set; }
    public int MedicalCenterId { get; set; }
    public int? DonorId { get; set; }
    public DateTime? CollectionDate { get; set; }
    public string? BatchNumber { get; set; }
    
    // Nếu không set thì mặc định 42 ngày từ ngày thu
    public DateTime? ExpiryDate { get; set; }
}

public class BloodInventoryStatisticsDto
{
    public int TotalUnits { get; set; }
    public decimal TotalVolume { get; set; }
    public int AvailableUnits { get; set; } 
    public int ReservedUnits { get; set; }
    public int UsedUnits { get; set; }
    public int ExpiredUnits { get; set; }
    public int ExpiringInWeek { get; set; }
    
    // Thống kê theo nhóm máu
    public List<BloodTypeStatistic> BloodTypeStatistics { get; set; } = new();
    
    public class BloodTypeStatistic
    {
        public string BloodType { get; set; } = string.Empty;
        public int Units { get; set; }
        public decimal Volume { get; set; }
        public string StockLevel { get; set; } = string.Empty; // Low, Normal, High
    }
}