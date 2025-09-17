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

    