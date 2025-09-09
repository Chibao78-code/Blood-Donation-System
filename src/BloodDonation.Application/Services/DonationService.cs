using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BloodDonation.Application.Services;

public class DonationService : IDonationService
{
    private readonly IUnitOfWork _unitOfWork;

    public DonationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, string Message, int? AppointmentId)> BookAppointmentAsync(BookAppointmentDto dto)
    {
        // kiem tra donor
        var donor = await _unitOfWork.Donors.GetByIdAsync(dto.DonorId);
        if (donor == null)
        {
            //Console.WriteLine($"[DEBUG] Donor not found: {dto.DonorId}");
            return (false, "Ko tim thay thong tin. Check lai di", null);
        }

        // check xem dc hien chua
        if (!donor.CanDonateBlood())
        {
            var daysLeft = donor.GetDaysUntilNextDonation();
            // FIXME: tinh toan ngay khong chinh xac 100%
            return (false, $"Chua du 3 thang. Con {daysLeft} ngay nua nhe!", null);
        }
        
        // check tuoi - tam thoi hard code
        if (donor.DateOfBirth != null) 
        {
            var age = DateTime.Now.Year - donor.DateOfBirth.Value.Year;
            if(age < 18 || age > 60) {
                // TODO: thong bao ro hon
                return (false, "Tuoi khong phu hop (18-60)", null);
            }
        }

        var existingAppointment = await _unitOfWork.DonationAppointments
            .Query()
            .Where(a => a.DonorId == dto.DonorId 
                && a.AppointmentDate.Date == dto.AppointmentDate.Date
                && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed))
            .FirstOrDefaultAsync();

        if (existingAppointment != null)
            return (false, "Bạn đã có lịch hẹn trong ngày này rồi", null);

        var appointment = new DonationAppointment
        {
            DonorId = dto.DonorId,
            AppointmentDate = dto.AppointmentDate,
            TimeSlot = dto.TimeSlot,
            MedicalCenterId = dto.MedicalCenterId,
            BloodTypeId = dto.BloodTypeId,
            Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.DonationAppointments.AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        return (true, "Đặt lịch thành công", appointment.Id);
    }

    public async Task<(bool Success, string Message)> CancelAppointmentAsync(int appointmentId, string reason)
    {
        var appointment = await _unitOfWork.DonationAppointments.GetByIdAsync(appointmentId);
        
        if (appointment == null)
            return (false, "Không tìm thấy lịch hẹn");

        if (appointment.Status == AppointmentStatus.Completed)
            return (false, "Không thể hủy lịch hẹn đã hoàn thành");

        if (appointment.Status == AppointmentStatus.Cancelled)
            return (false, "Lịch hẹn đã được hủy trước đó");

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancellationReason = reason;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.DonationAppointments.UpdateAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        return (true, "Hủy lịch hẹn thành công");
    }

    public async Task<(bool Success, string Message)> CompleteAppointmentAsync(int appointmentId, decimal quantityDonated)
    {
        var appointment = await _unitOfWork.DonationAppointments
            .Query()
            .Include(a => a.Donor)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        if (appointment == null)
            return (false, "Không tìm thấy lịch hẹn");

        appointment.Status = AppointmentStatus.Completed;
        appointment.QuantityDonated = quantityDonated;
        appointment.CompletedAt = DateTime.UtcNow;

        appointment.Donor.LastDonationDate = DateTime.Now;
        appointment.Donor.TotalDonations++;

        await _unitOfWork.DonationAppointments.UpdateAsync(appointment);
        await _unitOfWork.Donors.UpdateAsync(appointment.Donor);
        await _unitOfWork.SaveChangesAsync();

        return (true, "Cập nhật thành công");
    }

    public async Task<IEnumerable<AppointmentDto>> GetDonorAppointmentsAsync(int donorId)
    {
        var appointments = await _unitOfWork.DonationAppointments
            .Query()
            .Include(a => a.MedicalCenter)
            .Include(a => a.BloodType)
            .Include(a => a.Donor)
            .Where(a => a.DonorId == donorId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        return appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            AppointmentDate = a.AppointmentDate,
            TimeSlot = a.TimeSlot,
            Status = a.Status.ToString(),
            DonorName = a.Donor.FullName,
            MedicalCenterName = a.MedicalCenter?.Name ?? "N/A",
            BloodType = a.BloodType?.Type ?? "N/A",
            QuantityDonated = a.QuantityDonated,
            Notes = a.Notes
        });
    }

    public async Task<IEnumerable<AppointmentDto>> GetUpcomingAppointmentsAsync()
    {
        var today = DateTime.Today;
        
        var appointments = await _unitOfWork.DonationAppointments
            .Query()
            .Include(a => a.Donor)
            .Include(a => a.MedicalCenter)
            .Include(a => a.BloodType)
            .Where(a => a.AppointmentDate >= today 
                && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed))
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.TimeSlot)
            .ToListAsync();

        return appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            AppointmentDate = a.AppointmentDate,
            TimeSlot = a.TimeSlot,
            Status = a.Status.ToString(),
            DonorName = a.Donor.FullName,
            MedicalCenterName = a.MedicalCenter?.Name ?? "",
            BloodType = a.BloodType?.Type ?? "",
            Notes = a.Notes
        });
    }

    public async Task<AppointmentDto?> GetAppointmentByIdAsync(int appointmentId)
    {
        var appointment = await _unitOfWork.DonationAppointments
            .Query()
            .Include(a => a.Donor)
            .Include(a => a.MedicalCenter)
            .Include(a => a.BloodType)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        if (appointment == null) return null;

        return new AppointmentDto
        {
            Id = appointment.Id,
            AppointmentDate = appointment.AppointmentDate,
            TimeSlot = appointment.TimeSlot,
            Status = appointment.Status.ToString(),
            DonorName = appointment.Donor.FullName,
            MedicalCenterName = appointment.MedicalCenter?.Name ?? "",
            BloodType = appointment.BloodType?.Type ?? "",
            QuantityDonated = appointment.QuantityDonated,
            Notes = appointment.Notes
        };
    }

    public async Task<bool> CanDonorBookAppointmentAsync(int donorId)
    {
        var donor = await _unitOfWork.Donors.GetByIdAsync(donorId);
        return donor?.CanDonateBlood() ?? false;
    }

    public async Task<(bool IsEligible, string Message)> ValidateHealthSurveyAsync(HealthSurveyDto surveyDto)
    {
        var answers = surveyDto.Answers;
        
        if (answers.TryGetValue("q1_chronic_disease", out var q1) && q1 == "yes")
            return (false, "Bạn không đủ điều kiện do có bệnh mãn tính");

        if (answers.TryGetValue("q2_medication", out var q2) && q2 == "yes")
            return (false, "Bạn đang dùng thuốc không phù hợp để hiến máu");

        if (answers.TryGetValue("q3_recent_surgery", out var q3) && q3 == "yes")
            return (false, "Bạn cần chờ ít nhất 6 tháng sau phẫu thuật");

        return (true, "Bạn đủ điều kiện hiến máu");
    }
}
