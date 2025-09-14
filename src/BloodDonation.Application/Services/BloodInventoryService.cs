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
   