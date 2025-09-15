using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodDonation.Application.Services;

public class BloodInventoryService : IBloodInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BloodInventoryService> _logger;
    private readonly IEmailService _emailService;

    public BloodInventoryService(
        IUnitOfWork unitOfWork, 
        ILogger<BloodInventoryService> logger,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<List<BloodInventoryDto>> GetAvailableBloodAsync()
    {
        try
        {
            // Lấy tất cả máu còn sẵn sàng sử dụng
            var inventories = await _unitOfWork.BloodInventories
                .Query()
                .Include(b => b.BloodType)
                .Include(b => b.MedicalCenter)
                .Include(b => b.Donor)
                .Where(b => b.Status == BloodInventoryStatus.Available && 
                           b.ExpiryDate > DateTime.Now)
                .OrderBy(b => b.ExpiryDate) // ưu tiên dùng máu sắp hết hạn trước
                .ToListAsync();

            return inventories.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách máu sẵn có");
            throw;
        }
    }

    public async Task<List<BloodInventoryDto>> GetBloodByTypeAsync(int bloodTypeId)
    {
        var inventories = await _unitOfWork.BloodInventories
            .Query()
            .Include(b => b.BloodType)
            .Include(b => b.MedicalCenter)
            .Where(b => b.BloodTypeId == bloodTypeId && 
                       b.Status == BloodInventoryStatus.Available)
            .ToListAsync();

        return inventories.Select(MapToDto).ToList();
    }

    public async Task<BloodInventoryDto?> GetBloodInventoryByIdAsync(int id)
    {
        var inventory = await _unitOfWork.BloodInventories
            .Query()
            .Include(b => b.BloodType)
            .Include(b => b.MedicalCenter)
            .Include(b => b.Donor)
            .FirstOrDefaultAsync(b => b.Id == id);

        return inventory != null ? MapToDto(inventory) : null;
    }
    public async Task<BloodInventoryDto> AddBloodToInventoryAsync(AddBloodInventoryDto dto)
    {
        try
        {
            // Tạo mới túi máu
            var inventory = new BloodInventory
            {
                Quantity = dto.Quantity,
                BloodTypeId = dto.BloodTypeId,
                MedicalCenterId = dto.MedicalCenterId,
                DonorId = dto.DonorId,
                CollectionDate = dto.CollectionDate ?? DateTime.Now,
                // Mặc định hạn sử dụng là 42 ngày
                ExpiryDate = dto.ExpiryDate ?? (dto.CollectionDate ?? DateTime.Now).AddDays(42),
                Status = BloodInventoryStatus.Testing, // Ban đầu phải kiểm tra
                BatchNumber = dto.BatchNumber,
                CreatedAt = DateTime.Now
            };

            // Tự động tạo mã lô nếu chưa có
            inventory.GenerateBatchNumber();

            await _unitOfWork.BloodInventories.AddAsync(inventory);
            
            // Cập nhật thông tin người hiến nếu có
            if (dto.DonorId.HasValue)
            {
                var donor = await _unitOfWork.Donors.GetByIdAsync(dto.DonorId.Value);
                if (donor != null)
                {
                    donor.UpdateAfterDonation((int)dto.Quantity);
                    await _unitOfWork.Donors.UpdateAsync(donor);
                    
                    // Gửi email cảm ơn
                    if (donor.User != null)
                    {
                        await _emailService.SendDonationThankYouAsync(
                            donor.User.Email, 
                            donor.FullName);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            // Load lại với đầy đủ thông tin
            var result = await GetBloodInventoryByIdAsync(inventory.Id);
            return result!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thêm máu vào kho");
            throw;
        }
    }
        public async Task<bool> ReserveBloodAsync(int inventoryId, int requestId)
    {
        try
        {
            var inventory = await _unitOfWork.BloodInventories.GetByIdAsync(inventoryId);
            if (inventory == null)
            {
                _logger.LogWarning($"Không tìm thấy túi máu với ID {inventoryId}");
                return false;
            }

            // Kiểm tra xem có thể đặt trước không
            if (!inventory.IsUsable())
            {
                _logger.LogWarning($"Túi máu {inventoryId} không thể đặt trước");
                return false;
            }

            inventory.Reserve();
            await _unitOfWork.BloodInventories.UpdateAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Đã đặt trước túi máu {inventoryId} cho yêu cầu {requestId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi đặt trước túi máu {inventoryId}");
            return false;
        }
    }

    public async Task<bool> UseBloodAsync(int inventoryId)
    {
        try
        {
            var inventory = await _unitOfWork.BloodInventories.GetByIdAsync(inventoryId);
            if (inventory == null) return false;

            inventory.MarkAsUsed();
            await _unitOfWork.BloodInventories.UpdateAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Túi máu {inventoryId} đã được sử dụng");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi đánh dấu sử dụng túi máu {inventoryId}");
            return false;
        }
    }
    
    public async Task<bool> CancelReservationAsync(int inventoryId)
    {
        try
        {
            var inventory = await _unitOfWork.BloodInventories.GetByIdAsync(inventoryId);
            if (inventory == null) return false;

            inventory.CancelReservation();
            await _unitOfWork.BloodInventories.UpdateAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi hủy đặt trước túi máu {inventoryId}");
            return false;
        }
    }

    public async Task<int> CheckAndUpdateExpiredBloodAsync()
    {
        try
        {
            // Tìm tất cả máu đã hết hạn nhưng chưa được đánh dấu
            var expiredBlood = await _unitOfWork.BloodInventories
                .Query()
                .Where(b => b.ExpiryDate <= DateTime.Now && 
                           b.Status != BloodInventoryStatus.Expired &&
                           b.Status != BloodInventoryStatus.Used)
                .ToListAsync();

            if (!expiredBlood.Any()) return 0;

            // Cập nhật trạng thái
            foreach (var blood in expiredBlood)
            {
                blood.Status = BloodInventoryStatus.Expired;
                blood.UpdatedAt = DateTime.Now;
                await _unitOfWork.BloodInventories.UpdateAsync(blood);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Đã cập nhật {expiredBlood.Count} túi máu hết hạn");
            return expiredBlood.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi kiểm tra máu hết hạn");
            return 0;
        }
    }
   