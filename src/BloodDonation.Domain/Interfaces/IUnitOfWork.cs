using BloodDonation.Domain.Entities;

namespace BloodDonation.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Donor> Donors { get; }
    IRepository<MedicalCenter> MedicalCenters { get; }
    IRepository<BloodType> BloodTypes { get; }
    IRepository<DonationAppointment> DonationAppointments { get; }
    IRepository<HealthSurvey> HealthSurveys { get; }
    IRepository<DonationCertificate> DonationCertificates { get; }
    IRepository<BloodRequest> BloodRequests { get; }
    IRepository<BloodInventory> BloodInventories { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<News> News { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
