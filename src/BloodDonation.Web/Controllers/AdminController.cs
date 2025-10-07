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
        
        // Lấy danh sách appointment sắp tới
        var upcomingAppointments = await _donationService.GetUpcomingAppointmentsAsync();
        
        return View(upcomingAppointments);
    }

    // Quản lý người dùng
    public async Task<IActionResult> Users(int page = 1, int pageSize = 20)
    {
        if (!IsAdmin()) 
            return RedirectToAction("AccessDenied", "Account");

        var users = await _unitOfWork.Users
            .Query()
            .Include(u => u.Donor)
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalUsers = await _unitOfWork.Users.Query().CountAsync();
        
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
        ViewBag.TotalUsers = totalUsers;

        return View(users);
    }
        // Quản lý lịch hẹn - xem tất cả
    public async Task<IActionResult> Appointments(string status = "all", int page = 1)
    {
        if (!IsAdmin())
            return RedirectToAction("AccessDenied", "Account");

        var query = _unitOfWork.DonationAppointments
            .Query()
            .Include(a => a.Donor)
            .Include(a => a.MedicalCenter)
            .Include(a => a.BloodType);

        // Lọc theo status nếu có
        if (status != "all" && Enum.TryParse<Domain.Enums.AppointmentStatus>(status, true, out var statusEnum))
        {
            query = query.Where(a => a.Status == statusEnum);
        }

        var appointments = await query
            .OrderByDescending(a => a.AppointmentDate)
            .Take(50) // Lấy 50 cái gần nhất thôi
            .ToListAsync();

        ViewBag.CurrentStatus = status;
        
        return View(appointments);
    }

    // Duyệt lịch hẹn (chuyển từ Pending sang Confirmed)
    [HttpPost]
    public async Task<IActionResult> ApproveAppointment(int id)
    {
        if (!IsAdmin())
            return Json(new { success = false, message = "Không có quyền" });

        var appointment = await _unitOfWork.DonationAppointments.GetByIdAsync(id);
        if (appointment == null)
            return Json(new { success = false, message = "Không tìm thấy lịch hẹn" });

        appointment.Status = Domain.Enums.AppointmentStatus.Confirmed;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.DonationAppointments.UpdateAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Appointment {id} approved by admin");

        return Json(new { success = true, message = "Đã duyệt lịch hẹn" });
    }
        // Quản lý kho máu
    public async Task<IActionResult> BloodInventory(int? bloodTypeId = null)
    {
        if (!IsAdmin())
            return RedirectToAction("AccessDenied", "Account");

        var query = _unitOfWork.BloodInventories
            .Query()
            .Include(b => b.BloodType)
            .Include(b => b.MedicalCenter);

        if (bloodTypeId.HasValue)
        {
            query = query.Where(b => b.BloodTypeId == bloodTypeId.Value);
        }

        var inventory = await query
            .OrderByDescending(b => b.CollectionDate)
            .ToListAsync();

        // Lấy danh sách blood types để filter
        ViewBag.BloodTypes = await _unitOfWork.BloodTypes.GetAllAsync();
        ViewBag.SelectedBloodType = bloodTypeId;

        // Thống kê theo nhóm máu
        var stats = await _unitOfWork.BloodInventories
            .Query()
            .Where(b => b.Status == Domain.Enums.BloodInventoryStatus.Available)
            .GroupBy(b => b.BloodType.Name)
            .Select(g => new { 
                BloodType = g.Key, 
                Quantity = g.Sum(b => b.Quantity),
                Count = g.Count()
            })
            .ToListAsync();

        ViewBag.Stats = stats;

        return View(inventory);
    }
    
    // Quản lý trung tâm y tế
    public async Task<IActionResult> MedicalCenters()
    {
        if (!IsAdmin())
            return RedirectToAction("AccessDenied", "Account");

        var centers = await _unitOfWork.MedicalCenters
            .Query()
            .OrderBy(m => m.Name)
            .ToListAsync();

        return View(centers);
    }

    // Thêm trung tâm y tế mới
    [HttpGet]
    public IActionResult CreateMedicalCenter()
    {
        if (!IsAdmin())
            return RedirectToAction("AccessDenied", "Account");

        return View();
    }

       