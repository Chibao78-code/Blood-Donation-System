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

   