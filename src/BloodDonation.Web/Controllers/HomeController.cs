using System.Diagnostics;
using BloodDonation.Web.Models;
using Microsoft.AspNetCore.Mvc;
using BloodDonation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        // lay thong ke
        ViewBag.TotalDonors = await _unitOfWork.Donors.CountAsync();
        ViewBag.TotalAppointments = await _unitOfWork.DonationAppointments.CountAsync();
        
        // danh sach benh vien dang active  
        var centers = await _unitOfWork.MedicalCenters
            .Query()
            .Where(m => m.IsActive)
            .ToListAsync();
            
        ViewBag.MedicalCenters=centers;  // quen format :))
        
        // test data
        //_logger.LogInformation($"Total donors: {ViewBag.TotalDonors}");
        //ViewBag.TestData = "Hello test";
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
