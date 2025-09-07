
using System;
using BloodDonation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BloodDonation.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250906140859_InitialCreate")]
    partial class InitialCreate
    {
        
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BloodDonation.Domain.Entities.BloodInventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BatchNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BloodTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CollectionDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("MedicalCenterId")
                        .HasColumnType("int");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BloodTypeId");

                    b.HasIndex("MedicalCenterId");

                    b.ToTable("BloodInventories");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.BloodRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BloodTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUrgent")
                        .HasColumnType("bit");

                    b.Property<int>("MedicalCenterId")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PatientName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("QuantityRequired")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RequiredBy")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BloodTypeId");

                    b.HasIndex("MedicalCenterId");

                    b.ToTable("BloodRequests");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.BloodType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("BloodTypes");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.DonationAppointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AppointmentDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("BloodTypeId")
                        .HasColumnType("int");

                    b.Property<string>("CancellationReason")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DonorId")
                        .HasColumnType("int");

                    b.Property<int?>("HealthSurveyId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("MedicalCenterId")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<decimal?>("QuantityDonated")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TimeSlot")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentDate");

                    b.HasIndex("BloodTypeId");

                    b.HasIndex("HealthSurveyId");

                    b.HasIndex("MedicalCenterId");

                    b.HasIndex("Status");

                    b.HasIndex("DonorId", "AppointmentDate");

                    b.ToTable("DonationAppointments", (string)null);
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.DonationCertificate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CertificateNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DonationAppointmentId")
                        .HasColumnType("int");

                    b.Property<int>("DonorId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("IssueDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("QuantityDonated")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DonationAppointmentId")
                        .IsUnique();

                    b.HasIndex("DonorId");

                    b.ToTable("DonationCertificates");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.Donor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("BloodTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdentificationNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastDonationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalDonations")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BloodTypeId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Donors");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.HealthSurvey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DonorId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEligible")
                        .HasColumnType("bit");

                    b.Property<string>("RejectionReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SurveyData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SurveyDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DonorId");

                    b.ToTable("HealthSurveys");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.MedicalCenter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan?>("ClosingTime")
                        .HasColumnType("time");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("District")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan?>("OpeningTime")
                        .HasColumnType("time");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MedicalCenters");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.News", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("PublishedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ViewCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("News");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ReadAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("MedicalCenterId")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordSalt")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RefreshTokenExpiry")
                        .HasColumnType("datetime2");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("MedicalCenterId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.BloodInventory", b =>
                {
                    b.HasOne("BloodDonation.Domain.Entities.BloodType", "BloodType")
                        .WithMany("BloodInventories")
                        .HasForeignKey("BloodTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BloodDonation.Domain.Entities.MedicalCenter", "MedicalCenter")
                        .WithMany("BloodInventories")
                        .HasForeignKey("MedicalCenterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BloodType");

                    b.Navigation("MedicalCenter");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.BloodRequest", b =>
                {
                    b.HasOne("BloodDonation.Domain.Entities.BloodType", "BloodType")
                        .WithMany("BloodRequests")
                        .HasForeignKey("BloodTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BloodDonation.Domain.Entities.MedicalCenter", "MedicalCenter")
                        .WithMany("BloodRequests")
                        .HasForeignKey("MedicalCenterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BloodType");

                    b.Navigation("MedicalCenter");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.DonationAppointment", b =>
                {
                    b.HasOne("BloodDonation.Domain.Entities.BloodType", "BloodType")
                        .WithMany("DonationAppointments")
                        .HasForeignKey("BloodTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BloodDonation.Domain.Entities.Donor", "Donor")
                        .WithMany("DonationAppointments")
                        .HasForeignKey("DonorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BloodDonation.Domain.Entities.HealthSurvey", "HealthSurvey")
                        .WithMany("DonationAppointments")
                        .HasForeignKey("HealthSurveyId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("BloodDonation.Domain.Entities.MedicalCenter", "MedicalCenter")
                        .WithMany("DonationAppointments")
                        .HasForeignKey("MedicalCenterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BloodType");

                    b.Navigation("Donor");

                    b.Navigation("HealthSurvey");

                    b.Navigation("MedicalCenter");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.DonationCertificate", b =>
                {
                    b.HasOne("BloodDonation.Domain.Entities.DonationAppointment", "DonationAppointment")
                        .WithOne("Certificate")
                        .HasForeignKey("BloodDonation.Domain.Entities.DonationCertificate", "DonationAppointmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BloodDonation.Domain.Entities.Donor", "Donor")
                        .WithMany("DonationCertificates")
                        .HasForeignKey("DonorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DonationAppointment");

                    b.Navigation("Donor");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.Donor", b =>
                {
                    b.HasOne("BloodDonation.Domain.Entities.BloodType", "BloodType")
                        .WithMany("Donors")
                        .HasForeignKey("BloodTypeId");

                    b.HasOne("BloodDonation.Domain.Entities.User", "User")
                        .WithOne("Donor")
                        .HasForeignKey("BloodDonation.Domain.Entities.Donor", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BloodType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.HealthSurvey", b =>
                {
                    b.HasOne("BloodDonation.Domain.Entities.Donor", "Donor")
                        .WithMany("HealthSurveys")
                        .HasForeignKey("DonorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Donor");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.Notification", b =>
                {
                    b.HasOne("BloodDonation.Domain.Entities.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.User", b =>
                {
                    b.HasOne("BloodDonation.Domain.Entities.MedicalCenter", "MedicalCenter")
                        .WithMany("Staff")
                        .HasForeignKey("MedicalCenterId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("MedicalCenter");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.BloodType", b =>
                {
                    b.Navigation("BloodInventories");

                    b.Navigation("BloodRequests");

                    b.Navigation("DonationAppointments");

                    b.Navigation("Donors");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.DonationAppointment", b =>
                {
                    b.Navigation("Certificate");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.Donor", b =>
                {
                    b.Navigation("DonationAppointments");

                    b.Navigation("DonationCertificates");

                    b.Navigation("HealthSurveys");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.HealthSurvey", b =>
                {
                    b.Navigation("DonationAppointments");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.MedicalCenter", b =>
                {
                    b.Navigation("BloodInventories");

                    b.Navigation("BloodRequests");

                    b.Navigation("DonationAppointments");

                    b.Navigation("Staff");
                });

            modelBuilder.Entity("BloodDonation.Domain.Entities.User", b =>
                {
                    b.Navigation("Donor");

                    b.Navigation("Notifications");
                });
#pragma warning restore 612, 618
        }
    }
}
