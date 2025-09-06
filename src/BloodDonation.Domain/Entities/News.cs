using BloodDonation.Domain.Common;

namespace BloodDonation.Domain.Entities;

public class News : BaseEntity
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? Summary { get; set; }
    public string? ImageUrl { get; set; }
    public string? Author { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; } = 0;
    public string Type { get; set; } = "news"; // news, blog, announcement
}
