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

    