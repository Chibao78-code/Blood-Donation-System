using Microsoft.AspNetCore.Mvc;
using BloodDonation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using BloodDonation.Application.DTOs;

namespace BloodDonation.Web.Controllers;

public class ProfileController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IUnitOfWork unitOfWork, ILogger<ProfileController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        // get user info
        var user = await _unitOfWork.Users
            .Query()
            .Include(u => u.Donor)
                .ThenInclude(d => d.BloodType)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            _logger.LogWarning($"User not found: {username}");
            return RedirectToAction("Login", "Account");
        }

        // map to dto - manual mapping for now, maybe use automapper later
        var profileDto = new ProfileDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.Donor?.FullName ?? "",
            DateOfBirth = user.Donor?.DateOfBirth,
            Gender = user.Donor?.Gender,
            PhoneNumber = user.Donor?.PhoneNumber,
            Address = user.Donor?.Address,
            IdentificationNumber = user.Donor?.IdentificationNumber,
            BloodType = user.Donor?.BloodType?.Type,
            TotalDonations = user.Donor?.TotalDonations ?? 0,
            LastDonationDate = user.Donor?.LastDonationDate,
            IsAvailable = user.Donor?.IsAvailable ?? false
        };

        // calculate stats
        if(user.Donor != null)
        {
            ViewBag.CanDonateNow = user.Donor.CanDonateBlood();
            ViewBag.DaysUntilNext = user.Donor.GetDaysUntilNextDonation();
        }

        return View(profileDto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var username = HttpContext.Session.GetString("Username");
        if(string.IsNullOrEmpty(username))
            return RedirectToAction("Login", "Account");

        var user = await _unitOfWork.Users
            .Query()
            .Include(u => u.Donor)
            .FirstOrDefaultAsync(u => u.Username == username);

        if(user?.Donor == null)
        {
            // create new donor if not exists - weird case but handle it anyway
            _logger.LogInformation($"Creating donor profile for user: {username}");
            
            var donor = new Domain.Entities.Donor
            {
                UserId = user.Id,
                FullName = "",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
            
            await _unitOfWork.Donors.AddAsync(donor);
            await _unitOfWork.SaveChangesAsync();
            
            user.Donor = donor;
        }

        var editDto = new EditProfileDto
        {
            FullName = user.Donor.FullName,
            DateOfBirth = user.Donor.DateOfBirth,
            Gender = user.Donor.Gender,
            PhoneNumber = user.Donor.PhoneNumber,
            Address = user.Donor.Address,
            IdentificationNumber = user.Donor.IdentificationNumber,
            BloodTypeId = user.Donor.BloodTypeId
        };

        // load blood types for dropdown
        ViewBag.BloodTypes = await _unitOfWork.BloodTypes.GetAllAsync();
        
        return View(editDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.BloodTypes = await _unitOfWork.BloodTypes.GetAllAsync();
            return View(model);
        }

        var username = HttpContext.Session.GetString("Username");
        var user = await _unitOfWork.Users
            .Query()
            .Include(u => u.Donor)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user?.Donor == null)
        {
            TempData["Error"] = "Không tìm thấy thông tin người dùng";
            return RedirectToAction("Index");
        }

        // update donor info
        user.Donor.FullName = model.FullName ?? "";
        user.Donor.DateOfBirth = model.DateOfBirth;
        user.Donor.Gender = model.Gender;
        user.Donor.PhoneNumber = model.PhoneNumber;
        user.Donor.Address = model.Address;
        user.Donor.IdentificationNumber = model.IdentificationNumber;
        user.Donor.BloodTypeId = model.BloodTypeId;
        user.Donor.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.Donors.UpdateAsync(user.Donor);
            await _unitOfWork.SaveChangesAsync();
            
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile");
            TempData["Error"] = "Có lỗi xảy ra khi cập nhật thông tin";
            
            ViewBag.BloodTypes = await _unitOfWork.BloodTypes.GetAllAsync();
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleAvailability()
    {
        var username = HttpContext.Session.GetString("Username");
        var user = await _unitOfWork.Users
            .Query()
            .Include(u => u.Donor)
            .FirstOrDefaultAsync(u => u.Username == username);

        if(user?.Donor == null)
        {
            return Json(new { success = false, message = "Không tìm thấy thông tin" });
        }

        user.Donor.IsAvailable = !user.Donor.IsAvailable;
        await _unitOfWork.Donors.UpdateAsync(user.Donor);
        await _unitOfWork.SaveChangesAsync();

        var status = user.Donor.IsAvailable ? "sẵn sàng" : "không sẵn sàng";
        return Json(new { success = true, isAvailable = user.Donor.IsAvailable, message = $"Đã cập nhật trạng thái: {status}" });
    }
}
