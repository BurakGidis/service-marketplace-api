using Microsoft.EntityFrameworkCore;
using ServiceMarketAPI.Data;
using ServiceMarketAPI.DTOs.Appointments;
using ServiceMarketAPI.Models;

namespace ServiceMarketAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<object>> GetProviderAppointmentsAsync(string userId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.ServiceListing)
                .Include(a => a.Customer)
                .Where(a => a.ServiceListing.UserId == userId)
                .OrderByDescending(a => a.Date)
                .Select(a => new 
                {
                    a.Id,
                    ServiceName = a.ServiceListing.Title,
                    CustomerName = a.Customer.UserName,
                    Date = a.Date,
                    Status = a.Status.ToString()
                })
                .ToListAsync<object>();

            return appointments;
        }

        public async Task<(bool Success, string Message, int? AppointmentId)> CreateAppointmentAsync(CreateAppointmentRequest request, string userId)
        {
            if (request.Date < DateTime.UtcNow)
                return (false, "Geçmiş bir tarihe randevu alamazsınız.", null);

            var isBusy = await _context.Appointments.AnyAsync(a =>
                a.ServiceListingId == request.ServiceListingId && 
                a.Date == DateTime.SpecifyKind(request.Date, DateTimeKind.Utc) &&
                (a.Status == AppointmentStatus.Approved || a.Status == AppointmentStatus.Pending));
            
            if (isBusy)
                return (false, "Seçilen tarih ve saatte hizmet vermek mümkün değil.", null);

            var appointment = new Appointment
            {
                CustomerId = userId,
                ServiceListingId = request.ServiceListingId,
                Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
                Status = AppointmentStatus.Pending
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return (true, "Randevu talebi oluşturuldu.", appointment.Id);
        }

        public async Task<(bool Success, string Message)> ApproveAppointmentAsync(int id, string userId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.ServiceListing)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (appointment == null) return (false, "Randevu bulunamadı.");

            if (appointment.ServiceListing.UserId != userId)
                return (false, "Bu randevuyu onaylama yetkiniz yok.");
            
            if (appointment.Status != AppointmentStatus.Pending)
                return (false, $"Sadece 'Beklemede' olan randevular onaylanabilir. Şu anki durum: {appointment.Status}");

            appointment.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync();

            return (true, "Randevu onaylandı.");
        }

        public async Task<(bool Success, string Message)> RejectAppointmentAsync(int id, string userId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.ServiceListing)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return (false, "Randevu bulunamadı.");

            if (appointment.ServiceListing.UserId != userId)
                return (false, "Yetkiniz yok.");

            if (appointment.Status == AppointmentStatus.Completed)
                return (false, "Tamamlanmış bir randevu reddedilemez.");

            appointment.Status = AppointmentStatus.Rejected;
            await _context.SaveChangesAsync();

            return (true, "Randevu reddedildi.");
        }

        public async Task<(bool Success, string Message)> CompleteAppointmentAsync(int id, string userId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.ServiceListing)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return (false, "Randevu bulunamadı.");

            if (appointment.ServiceListing.UserId != userId)
                return (false, "Yetkiniz yok.");

            if (appointment.Status != AppointmentStatus.Approved)
                return (false, "Sadece onaylanmış randevular tamamlandı olarak işaretlenebilir.");

            appointment.Status = AppointmentStatus.Completed;
            await _context.SaveChangesAsync();

            return (true, "Randevu tamamlandı.");
        }

        public async Task<(bool Success, string Message)> CancelAppointmentAsync(int id, string userId)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return (false, "Randevu bulunamadı.");
            
            if (appointment.CustomerId != userId)
                return (false, "Başkasına ait bir randevuyu iptal edemezsiniz.");

            if (appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.Rejected)
                return (false, "Bu randevu artık iptal edilemez.");

            appointment.Status = AppointmentStatus.Canceled;
            await _context.SaveChangesAsync();

            return (true, "Randevunuz iptal edildi.");
        }
    }
}