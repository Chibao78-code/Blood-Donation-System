namespace BloodDonation.Domain.Enums;

/// <summary>
/// Trạng thái của túi máu trong kho
/// </summary>
public enum BloodInventoryStatus
{
    /// <summary>
    /// Sẵn sàng để sử dụng
    /// </summary>
    Available = 1,
    
    /// <summary>
    /// Đã được đặt trước cho bệnh nhân cụ thể
    /// </summary>
    Reserved = 2,
    
    /// <summary>
    /// Đã sử dụng cho truyền máu
    /// </summary>
    Used = 3,
    
    /// <summary>
    /// Đã hết hạn sử dụng
    /// </summary>
    Expired = 4,
    
    /// <summary>
    /// Đang trong quá trình kiểm tra chất lượng
    /// </summary>
    Testing = 5,
    
    /// <summary>
    /// Không đạt tiêu chuẩn, cần hủy
    /// </summary>
    Rejected = 6
}