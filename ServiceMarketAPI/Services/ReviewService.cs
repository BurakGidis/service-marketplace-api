using Microsoft.EntityFrameworkCore;
using ServiceMarketAPI.Data;
using ServiceMarketAPI.DTOs.Appointments;
using ServiceMarketAPI.Models;

namespace ServiceMarketAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> AddReviewAsync(CreateReviewRequest request, string userId)
        {
            // Randevuyu kontrol et
            var appointment = await _context.Appointments
                .Include(a => a.ServiceListing)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId);
            
            if (appointment == null) 
                return (false, "Randevu bulunamadı.");
            
            //  Kullanıcı yetkisi kontrolü
            if (appointment.CustomerId != userId) 
                return (false, "Sadece randevuyu alan müşteri yorum yapabilir.");
            
            //  Randevu durumu kontrolü
            if (appointment.Status != AppointmentStatus.Completed) 
                return (false, "Hizmet tamamlanmadan yorum yapılamaz.");
            
            //  Daha önce yorum yapılmış mı?
            var existingReview = await _context.Reviews
                .AnyAsync(r => r.AppointmentId == request.AppointmentId);
            
            if (existingReview)
                return (false, "Bu randevu için zaten bir değerlendirme yapılmış.");
            
            //  Yorumu oluştur ve kaydet
            var review = new Review
            {
                AppointmentId = request.AppointmentId,
                Rating = request.Rating,
                Comment = request.Comment ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            //  Hizmetin ortalama puanını güncelle
            var allReviews = await _context.Reviews
                .Include(r => r.Appointment)
                .Where(r => r.Appointment.ServiceListingId == appointment.ServiceListingId)
                .ToListAsync();
            
            if (allReviews.Any())
            {
                double newAverage = allReviews.Average(r => r.Rating);
                appointment.ServiceListing.AverageRating = newAverage;
                await _context.SaveChangesAsync();
            }

            return (true, "Yorum başarıyla eklendi ve hizmet puanı güncellendi.");
        }
    }
}