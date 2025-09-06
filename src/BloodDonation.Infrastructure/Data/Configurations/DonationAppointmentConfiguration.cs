using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Infrastructure.Data.Configurations;

public class DonationAppointmentConfiguration : IEntityTypeConfiguration<DonationAppointment>
{
    public void Configure(EntityTypeBuilder<DonationAppointment> builder)
    {
        builder.ToTable("DonationAppointments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.AppointmentDate)
            .IsRequired();

        builder.Property(d => d.TimeSlot)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.Status)
            .IsRequired();

        builder.Property(d => d.QuantityDonated)
            .HasPrecision(10, 2);

        builder.Property(d => d.Notes)
            .HasMaxLength(500);

        builder.Property(d => d.CancellationReason)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(d => d.AppointmentDate);
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => new { d.DonorId, d.AppointmentDate });

        // Relationships
        builder.HasOne(d => d.Donor)
            .WithMany(donor => donor.DonationAppointments)
            .HasForeignKey(d => d.DonorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.MedicalCenter)
            .WithMany(m => m.DonationAppointments)
            .HasForeignKey(d => d.MedicalCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.BloodType)
            .WithMany(b => b.DonationAppointments)
            .HasForeignKey(d => d.BloodTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.HealthSurvey)
            .WithMany(h => h.DonationAppointments)
            .HasForeignKey(d => d.HealthSurveyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Certificate)
            .WithOne(c => c.DonationAppointment)
            .HasForeignKey<DonationCertificate>(c => c.DonationAppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
