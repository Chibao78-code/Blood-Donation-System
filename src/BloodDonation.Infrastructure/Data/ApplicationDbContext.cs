using Microsoft.EntityFrameworkCore;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Common;
using System.Reflection;
using System.Linq.Expressions;

namespace BloodDonation.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Donor> Donors { get; set; }
    public DbSet<MedicalCenter> MedicalCenters { get; set; }
    public DbSet<BloodType> BloodTypes { get; set; }
    public DbSet<DonationAppointment> DonationAppointments { get; set; }
    public DbSet<HealthSurvey> HealthSurveys { get; set; }
    public DbSet<DonationCertificate> DonationCertificates { get; set; }
    public DbSet<BloodRequest> BloodRequests { get; set; }
    public DbSet<BloodInventory> BloodInventories { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<News> News { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // load 
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        //  add thêm nếu cần thiết 
        
        // chi lay record chua bi xoa
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var param = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(param, nameof(BaseEntity.IsDeleted));
                var filter = Expression.Lambda(
                    Expression.Equal(prop, Expression.Constant(false)),
                    param);
                    
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    // Soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
