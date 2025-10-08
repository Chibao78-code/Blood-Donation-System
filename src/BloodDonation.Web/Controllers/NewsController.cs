using Microsoft.AspNetCore.Mvc;
using BloodDonation.Domain.Interfaces;
using BloodDonation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Web.Controllers;

public class NewsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NewsController> _logger;

    public NewsController(IUnitOfWork unitOfWork, ILogger<NewsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // Trang danh sách tin tức (public - ai cũng xem được)
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        // Chỉ lấy các bài đã publish
        var query = _unitOfWork.News
            .Query()
            .Where(n => n.IsPublished)
            .OrderByDescending(n => n.PublishedAt ?? n.CreatedAt);

        var totalNews = await query.CountAsync();
        
        var news = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalNews / (double)pageSize);
        ViewBag.TotalNews = totalNews;

        return View(news);
    }
        // Chi tiết bài viết
    public async Task<IActionResult> Details(int id)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);

        if (news == null || !news.IsPublished)
        {
            TempData["Error"] = "Không tìm thấy bài viết này";
            return RedirectToAction("Index");
        }

        // Tăng view count
        news.ViewCount++;
        await _unitOfWork.News.UpdateAsync(news);
        await _unitOfWork.SaveChangesAsync();

        // Lấy các bài viết liên quan (cùng type)
        var relatedNews = await _unitOfWork.News
            .Query()
            .Where(n => n.IsPublished && n.Id != id && n.Type == news.Type)
            .OrderByDescending(n => n.PublishedAt)
            .Take(5)
            .ToListAsync();

        ViewBag.RelatedNews = relatedNews;

        return View(news);
    }

    // Admin: Quản lý tin tức
    public async Task<IActionResult> Manage()
    {
        // Check quyền admin
        var role = HttpContext.Session.GetString("Role");
        if (role?.ToLower() != "admin")
        {
            TempData["Error"] = "Bạn không có quyền truy cập";
            return RedirectToAction("Index", "Home");
        }

        // Lấy tất cả tin tức (kể cả chưa publish)
        var news = await _unitOfWork.News
            .Query()
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return View(news);
    }
    
    // Tạo bài viết mới
    [HttpGet]
    public IActionResult Create()
    {
        var role = HttpContext.Session.GetString("Role");
        if (role?.ToLower() != "admin")
            return RedirectToAction("AccessDenied", "Account");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(News model)
    {
        var role = HttpContext.Session.GetString("Role");
        if (role?.ToLower() != "admin")
            return RedirectToAction("AccessDenied", "Account");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            // Set author từ session
            var username = HttpContext.Session.GetString("Username");
            model.Author = username ?? "Admin";

            // Nếu đánh dấu publish ngay
            if (model.IsPublished)
            {
                model.PublishedAt = DateTime.UtcNow;
            }

            await _unitOfWork.News.AddAsync(model);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Đã tạo bài viết mới";
            return RedirectToAction("Manage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating news");
            ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            return View(model);
        }
    }

    