using BloodDonation.Application.DTOs;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces;

public interface IBloodInventoryService
{
    // Lấy danh sách tồn kho máu
    Task<List<BloodInventoryDto>> GetAvailableBloodAsync();
    Task<List<BloodInventoryDto>> GetBloodByTypeAsync(int bloodTypeId);
    Task<BloodInventoryDto?> GetBloodInventoryByIdAsync(int id);
    
    // Thêm máu vào kho sau khi nhận từ người hiến
    Task<BloodInventoryDto> AddBloodToInventoryAsync(AddBloodInventoryDto dto);
    
    // Cập nhật trạng thái túi máu
    Task<bool> ReserveBloodAsync(int inventoryId, int requestId);
    Task<bool> UseBloodAsync(int inventoryId);
    Task<bool> CancelReservationAsync(int inventoryId);
    
    // Kiểm tra và xử lý máu hết hạn
    Task<int> CheckAndUpdateExpiredBloodAsync();
    
    // Thống kê
    Task<BloodInventoryStatisticsDto> GetStatisticsAsync();
    Task<List<BloodInventoryDto>> GetExpiringBloodAsync(int daysAhead = 7);
    Task<List<BloodInventoryDto>> GetLowStockBloodTypesAsync(int minimumUnits = 5);
    
    // Tìm kiếm máu phù hợp cho bệnh nhân
    Task<List<BloodInventoryDto>> FindCompatibleBloodAsync(string bloodType, decimal quantityNeeded);
}