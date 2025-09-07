using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class Notification : BaseEntity
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string Type { get; set; } = "Info"; // Info, Warning, Success, Error
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
