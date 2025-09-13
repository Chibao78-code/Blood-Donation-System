using BloodDonation.Domain.Common;
using BloodDonation.Domain.Enums;

namespace BloodDonation.Domain.Entities;

public class BloodInventory : BaseEntity
{
    private decimal _quantity;
    private string? _batchNumber;
    
    /// <summary>
    /// Lượng máu (ml) - thường là 250ml, 350ml hoặc 450ml
    /// </summary>
    public decimal Quantity 
    { 
        get => _quantity;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Lượng máu phải lớn hơn 0");
                
            _quantity = value;
        }
    }
    
    /// <summary>
    /// Ngày thu máu
    /// </summary>
    public DateTime CollectionDate { get; set; }
    
    /// <summary>
    /// Ngày hết hạn (thường là 42 ngày sau khi thu)
    /// </summary>
    public DateTime ExpiryDate { get; set; }
    
    /// <summary>
    /// Trạng thái hiện tại của túi máu
    /// </summary>
    public BloodInventoryStatus Status { get; set; } = BloodInventoryStatus.Testing;
    
    /// <summary>
    /// Mã lô sản xuất/thu máu
    /// </summary>
    public string? BatchNumber 
    { 
        get => _batchNumber;
        set => _batchNumber = value?.Trim().ToUpper();
    }
    
    /// <summary>
    /// Nhiệt độ bảo quản yêu cầu (thường là 2-6°C)
    /// </summary>
    public string StorageTemperature { get; set; } = "2-6°C";
    
    /// <summary>
    /// Người hiến máu (nếu biết)
    /// </summary>
    public int? DonorId { get; set; }
    public virtual Donor? Donor { get; set; }

    public int MedicalCenterId { get; set; }
    public int BloodTypeId { get; set; }

    public virtual MedicalCenter MedicalCenter { get; set; } = null!;
    public virtual BloodType BloodType { get; set; } = null!;

    /// <summary>
    /// Kiểm tra xem túi máu đã hết hạn chưa
    /// </summary>
    public bool IsExpired()
    {
        return DateTime.Now > ExpiryDate;
    }
    
    /// <summary>
    /// Tính số ngày còn lại trước khi hết hạn
    /// </summary>
    public int DaysUntilExpiry()
    {
        var days = (ExpiryDate - DateTime.Now).Days;
        return days < 0 ? 0 : days;
    }
    
    /// <summary>
    /// Kiểm tra xem túi máu có sắp hết hạn không (trong vòng 7 ngày)
    /// </summary>
    public bool IsNearExpiry()
    {
        return DaysUntilExpiry() <= 7 && !IsExpired();
    }
    
    /// <summary>
    /// Kiểm tra xem túi máu có sẵn sàng để sử dụng không
    /// </summary>
    public bool IsUsable()
    {
        return Status == BloodInventoryStatus.Available && 
               !IsExpired();
    }
    
    /// <summary>
    /// Đặt trước túi máu cho bệnh nhân
    /// </summary>
    public void Reserve()
    {
        if (!IsUsable())
            throw new InvalidOperationException("Túi máu không sẵn sàng để đặt trước");
            
        Status = BloodInventoryStatus.Reserved;
        UpdatedAt = DateTime.Now;
    }
    
    /// <summary>
    /// Đánh dấu túi máu đã được sử dụng
    /// </summary>
    public void MarkAsUsed()
    {
        Status = BloodInventoryStatus.Used;
        UpdatedAt = DateTime.Now;
    }
    
    /// <summary>
    /// Hủy đặt trước và trả lại trạng thái sẵn sàng
    /// </summary>
    public void CancelReservation()
    {
        if (Status != BloodInventoryStatus.Reserved)
            throw new InvalidOperationException("Chỉ có thể hủy túi máu đã đặt trước");
            
        Status = BloodInventoryStatus.Available;
        UpdatedAt = DateTime.Now;
    }
    
    /// <summary>
    /// Tạo mã lô tự động nếu chưa có
    /// </summary>
    public void GenerateBatchNumber()
    {
        if (string.IsNullOrEmpty(BatchNumber))
        {
            // Format: BL-YYYYMMDD-XXXX (ví dụ: BL-20250113-0001)
            var date = CollectionDate.ToString("yyyyMMdd");
            var random = new Random().Next(1, 9999).ToString("D4");
            BatchNumber = $"BL-{date}-{random}";
        }
    }
}
