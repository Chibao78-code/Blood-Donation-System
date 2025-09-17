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


    