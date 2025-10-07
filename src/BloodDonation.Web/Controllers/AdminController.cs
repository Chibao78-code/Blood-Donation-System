using Microsoft.AspNetCore.Mvc;
using BloodDonation.Domain.Interfaces;
using BloodDonation.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Web.Controllers;

public class AdminController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDonationService _donationService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUnitOfWork unitOfWork,
        IDonationService donationService,
        ILogger<AdminController> logger)
    {
        _unitOfWork = unitOfWork;
        _donationService = donationService;
        _logger = logger;
    }

    // Kiểm tra xem user có phải admin không
    private bool IsAdmin()
    {
        var role = HttpContext.Session.GetString("Role");
        return role?.ToLower() == "admin";
    }

    public async Task<IActionResult> Dashboard()
    {
        // Check quyền admin
        if (!IsAdmin())
        {
            TempData["Error"] = "Bạn không có quyền truy cập trang này";
            return RedirectToAction("Index", "Home");
        }
         // Lấy thống kê tổng quan
        var totalDonors = await _unitOfWork.Donors.Query().CountAsync();
        var totalAppointments = await _unitOfWork.DonationAppointments.Query().CountAsync();
        var pendingAppointments = await _unitOfWork.DonationAppointments
            .Query()
            .Where(a => a.Status == Domain.Enums.AppointmentStatus.Pending)
            .CountAsync();

        // Lấy số lượng túi máu trong kho
        var totalBloodUnits = await _unitOfWork.BloodInventories
            .Query()
            .Where(b => b.Status == Domain.Enums.BloodInventoryStatus.Available)
            .SumAsync(b => (decimal?)b.Quantity) ?? 0;

        // Lấy số lượng yêu cầu máu đang chờ
        var pendingRequests = await _unitOfWork.BloodRequests
            .Query()
            .Where(r => r.Status == "Pending")
            .CountAsync();

        ViewBag.TotalDonors = totalDonors;
        ViewBag.TotalAppointments = totalAppointments;
        ViewBag.PendingAppointments = pendingAppointments;
        ViewBag.TotalBloodUnits = totalBloodUnits;
        ViewBag.PendingRequests = pendingRequests;

       