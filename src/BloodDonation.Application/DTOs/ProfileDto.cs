using System.ComponentModel.DataAnnotations;

namespace BloodDonation.Application.DTOs;

public class ProfileDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentificationNumber { get; set; }
    public string? BloodType { get; set; }
    public int TotalDonations { get; set; }
    public DateTime? LastDonationDate { get; set; }
    public bool IsAvailable { get; set; }
}

public class EditProfileDto
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? IdentificationNumber { get; set; }

    public int? BloodTypeId { get; set; }
}
