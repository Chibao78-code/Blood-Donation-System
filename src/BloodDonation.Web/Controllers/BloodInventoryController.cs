using Microsoft.AspNetCore.Mvc;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.DTOs;
using BloodDonation.Domain.Interfaces;
using BloodDonation.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Web.Controllers;

public class BloodInventoryController : Controller
{
    private readonly IBloodInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BloodInventoryController> _logger;

    public BloodInventoryController(
        IBloodInventoryService inventoryService,
        IUnitOfWork unitOfWork,
        ILogger<BloodInventoryController> logger)
    {
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    // Trang chủ quản lý kho máu
    public async Task<IActionResult> Index()
    {
        try
        {
            // Kiểm tra quyền - chỉ staff và admin mới xem được
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Staff" && userRole != "Admin")
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này";
                return RedirectToAction("Index", "Home");
            }

            // Lấy tất cả máu trong kho
            var inventories = await _inventoryService.GetAvailableBloodAsync();
            
            // Lấy thống kê
            var statistics = await _inventoryService.GetStatisticsAsync();
            ViewBag.Statistics = statistics;
            
            // Lấy danh sách máu sắp hết hạn
            var expiringBlood = await _inventoryService.GetExpiringBloodAsync(7);
            ViewBag.ExpiringBlood = expiringBlood;
            
            // Kiểm tra và cập nhật máu hết hạn
            await _inventoryService.CheckAndUpdateExpiredBloodAsync();
            
            return View(inventories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách kho máu");
            TempData["Error"] = "Có lỗi xảy ra khi tải dữ liệu";
            return View(new List<BloodInventoryDto>());
        }
    }
    
    // Chi tiết một túi máu
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var inventory = await _inventoryService.GetBloodInventoryByIdAsync(id);
        
        if (inventory == null)
        {
            TempData["Error"] = "Không tìm thấy thông tin túi máu";
            return RedirectToAction("Index");
        }

        return View(inventory);
    }

    // Form thêm máu vào kho
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        // Kiểm tra quyền
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole != "Staff" && userRole != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        // Load dữ liệu cho dropdown
        ViewBag.BloodTypes = await _unitOfWork.BloodTypes.GetAllAsync();
        ViewBag.MedicalCenters = await _unitOfWork.MedicalCenters
            .Query()
            .Where(m => m.IsActive)
            .ToListAsync();
            
        // Lấy danh sách người hiến gần đây để chọn
        ViewBag.RecentDonors = await _unitOfWork.Donors
            .Query()
            .Include(d => d.User)
            .Where(d => d.LastDonationDate != null && 
                       d.LastDonationDate > DateTime.Now.AddDays(-7))
            .OrderByDescending(d => d.LastDonationDate)
            .Take(20)
            .Select(d => new { d.Id, d.FullName, d.LastDonationDate })
            .ToListAsync();

        return View();
    }
    
    // Xử lý thêm máu vào kho
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddBloodInventoryDto model)
    {
        if (!ModelState.IsValid)
        {
            // Load lại dữ liệu nếu có lỗi
            ViewBag.BloodTypes = await _unitOfWork.BloodTypes.GetAllAsync();
            ViewBag.MedicalCenters = await _unitOfWork.MedicalCenters
                .Query()
                .Where(m => m.IsActive)
                .ToListAsync();
                
            return View(model);
        }

        try
        {
            var result = await _inventoryService.AddBloodToInventoryAsync(model);
            
            TempData["Success"] = $"Đã thêm túi máu {result.BatchNumber} vào kho";
            return RedirectToAction("Details", new { id = result.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thêm máu vào kho");
            TempData["Error"] = "Có lỗi xảy ra khi thêm máu vào kho";
            
            // Load lại dữ liệu
            ViewBag.BloodTypes = await _unitOfWork.BloodTypes.GetAllAsync();
            ViewBag.MedicalCenters = await _unitOfWork.MedicalCenters
                .Query()
                .Where(m => m.IsActive)
                .ToListAsync();
                
            return View(model);
        }
    }
        // Đặt trước túi máu
    [HttpPost]
    public async Task<IActionResult> Reserve(int inventoryId, int requestId)
    {
        var success = await _inventoryService.ReserveBloodAsync(inventoryId, requestId);
        
        if (success)
        {
            TempData["Success"] = "Đã đặt trước túi máu thành công";
        }
        else
        {
            TempData["Error"] = "Không thể đặt trước túi máu này";
        }
        
        return RedirectToAction("Index");
    }
}
