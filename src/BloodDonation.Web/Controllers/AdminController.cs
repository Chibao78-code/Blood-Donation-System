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

       