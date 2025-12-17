using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceMarketAPI.Data;
using ServiceMarketAPI.DTOs.Appointments;
using ServiceMarketAPI.Models;
using System.Security.Claims;

namespace ServiceMarketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostReview([FromBody] CreateReviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointment = await _context.Appointments
                .Include(a => a.ServiceListing)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId); 
            
            if (appointment == null) 
                return NotFound("No appointment found.");
            
            if (appointment.CustomerId != userId) 
                return Unauthorized("Only customers who booked the appointment can leave a review.");
            
            if (appointment.Status != AppointmentStatus.Completed) 
                return BadRequest("No comments can be made before the service is completed.");
            
            var existingReview = await _context.Reviews
                .AnyAsync(r => r.AppointmentId == request.AppointmentId);
            
            if (existingReview)
                return BadRequest("This appointment has already received a review.");
            
            var review = new Review
            {
                AppointmentId = request.AppointmentId,
                Rating = request.Rating,
                Comment = request.Comment ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);

            await _context.SaveChangesAsync();

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

            return Ok(new { message = "The review has been successfully added and the service rating has been updated." });
        
        }
    }
}