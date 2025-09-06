using Microsoft.EntityFrameworkCore.Storage;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;
using BloodDonation.Infrastructure.Data;

namespace BloodDonation.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    // Repository instances
    private IRepository<User>? _users;
    private IRepository<Donor>? _donors;
    private IRepository<MedicalCenter>? _medicalCenters;
    private IRepository<BloodType>? _bloodTypes;
    private IRepository<DonationAppointment>? _donationAppointments;
    private IRepository<HealthSurvey>? _healthSurveys;
    private IRepository<DonationCertificate>? _donationCertificates;
    private IRepository<BloodRequest>? _bloodRequests;
    private IRepository<BloodInventory>? _bloodInventories;
    private IRepository<Notification>? _notifications;
    private IRepository<News>? _news;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Repository properties with lazy initialization
    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Donor> Donors => _donors ??= new Repository<Donor>(_context);
    public IRepository<MedicalCenter> MedicalCenters => _medicalCenters ??= new Repository<MedicalCenter>(_context);
    public IRepository<BloodType> BloodTypes => _bloodTypes ??= new Repository<BloodType>(_context);
    public IRepository<DonationAppointment> DonationAppointments => _donationAppointments ??= new Repository<DonationAppointment>(_context);
    public IRepository<HealthSurvey> HealthSurveys => _healthSurveys ??= new Repository<HealthSurvey>(_context);
    public IRepository<DonationCertificate> DonationCertificates => _donationCertificates ??= new Repository<DonationCertificate>(_context);
    public IRepository<BloodRequest> BloodRequests => _bloodRequests ??= new Repository<BloodRequest>(_context);
    public IRepository<BloodInventory> BloodInventories => _bloodInventories ??= new Repository<BloodInventory>(_context);
    public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(_context);
    public IRepository<News> News => _news ??= new Repository<News>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
