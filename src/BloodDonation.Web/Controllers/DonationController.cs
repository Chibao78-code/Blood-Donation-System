using Microsoft.AspNetCore.Mvc;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.DTOs;
using BloodDonation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Web.Controllers;

public class DonationController : Controller
{
    private readonly IDonationService _donationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DonationController> _logger;

    public DonationController(
        IDonationService donationService, 
        IUnitOfWork unitOfWork,
        ILogger<DonationController> logger)
    {
        _donationService = donationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr))
            return RedirectToAction("Login", "Account");

        var userId = int.Parse(userIdStr);
        var donor = await _unitOfWork.Donors
            .Query()
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (donor == null)
        {
            TempData["Error"] = "Vui lòng cập nhật thông tin cá nhân trước khi đặt lịch";
            return RedirectToAction("Profile", "Account");
        }

        var appointments = await _donationService.GetDonorAppointmentsAsync(donor.Id);
        
        ViewBag.CanBookNew = await _donationService.CanDonorBookAppointmentAsync(donor.Id);
        ViewBag.DaysUntilNext = donor.GetDaysUntilNextDonation();
        
        return View(appointments);
    }

    [HttpGet]
    public async Task<IActionResult> Book()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr))
            return RedirectToAction("Login", "Account");

        var userId = int.Parse(userIdStr);
        var donor = await _unitOfWork.Donors
            .Query()
            .Include(d => d.BloodType)
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (donor == null)
        {
            TempData["Error"] = "Vui lòng cập nhật thông tin cá nhân";
            return RedirectToAction("Profile", "Account");
        }

        if (!donor.CanDonateBlood())
        {
            TempData["Error"] = $"Bạn cần chờ thêm {donor.GetDaysUntilNextDonation()} ngày";
            return RedirectToAction("Index");
        }

        ViewBag.MedicalCenters = await _unitOfWork.MedicalCenters
            .Query()
            .Where(m => m.IsActive)
            .ToListAsync();

        ViewBag.BloodTypes = await _unitOfWork.BloodTypes.GetAllAsync();
        ViewBag.DonorBloodTypeId = donor.BloodTypeId;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Book(CreateAppointmentDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.MedicalCenters = await _unitOfWork.MedicalCenters
                .Query()
                .Where(m => m.IsActive)
                .ToListAsync();
            ViewBag.BloodTypes = await _unitOfWork.BloodTypes.GetAllAsync();
            return View(model);
        }

        var userIdStr = HttpContext.Session.GetString("UserId");
        var userId = int.Parse(userIdStr);
        
        var donor = await _unitOfWork.Donors
            .Query()
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (donor == null)
        {
            TempData["Error"] = "Không tìm thấy thông tin";
            return RedirectToAction("Index");
        }

        var bookingDto = new BookAppointmentDto
        {
            DonorId = donor.Id,
            AppointmentDate = model.AppointmentDate,
            TimeSlot = model.TimeSlot,
            MedicalCenterId = model.MedicalCenterId,
            BloodTypeId = model.BloodTypeId
        };

        var result = await _donationService.BookAppointmentAsync(bookingDto);

        if (result.Success)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction("Index");
        }

        TempData["Error"] = result.Message;
        ViewBag.MedicalCenters = await _unitOfWork.MedicalCenters
            .Query()
            .Where(m => m.IsActive)
            .ToListAsync();
        ViewBag.BloodTypes = await _unitOfWork.BloodTypes.GetAllAsync();
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id, string reason)
    {
        var result = await _donationService.CancelAppointmentAsync(id, reason);
        
        if (result.Success)
            TempData["Success"] = result.Message;
        else
            TempData["Error"] = result.Message;

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var appointment = await _donationService.GetAppointmentByIdAsync(id);
        
        if (appointment == null)
        {
            TempData["Error"] = "Không tìm thấy lịch hẹn";
            return RedirectToAction("Index");
        }

        return View(appointment);
    }

    [HttpGet]
    public IActionResult HealthSurvey()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> HealthSurvey(HealthSurveyDto model)
    {
        var result = await _donationService.ValidateHealthSurveyAsync(model);
        
        if (result.IsEligible)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction("Book");
        }

        TempData["Error"] = result.Message;
        return View(model);
    }
}
