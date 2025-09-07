namespace BloodDonation.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }  
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    // soft delete, có thể thay đổi
    public bool IsDeleted { get; set; }
}
