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
    [Authorize] //only logined users can be make a transaction 
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("provider")]
        public async Task<IActionResult> GetProviderAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isBusy = await _context.Appointments.AnyAsync(a =>
                a.ServiceListingId == request.ServiceListingId && 
                a.Date == DateTime.SpecifyKind(request.Date, DateTimeKind.Utc) &&
                (a.Status == AppointmentStatus.Approved || a.Status == AppointmentStatus.Pending));
            
            if (isBusy)
            {
                return BadRequest("It is not possible to provide service at the selected date and time.");
            }
            var appointment = new Appointment
            {
                CustomerId = userId ?? string.Empty,
                ServiceListingId = request.ServiceListingId,
                Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
                Status = AppointmentStatus.Pending
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "An appointment request has been created.", id = appointment.Id });

        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointment = await _context.Appointments
                .Include(a => a.ServiceListing)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (appointment == null) return NotFound("No appointment found.");

            if (appointment.ServiceListing.UserId != userId)
                return Unauthorized("You do not have the authority to approve this appointment.");
            
            appointment.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync();

            return Ok("Appointment confirmed.");
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointment = await _context.Appointments
                .Include(a => a.ServiceListing)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound();

            if (appointment.ServiceListing.UserId != userId)
                return Unauthorized();

            appointment.Status = AppointmentStatus.Rejected;
            await _context.SaveChangesAsync();

            return Ok("The appointment was rejected.");
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointment = await _context.Appointments
                .Include(a => a.ServiceListing)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound();

            if (appointment.ServiceListing.UserId != userId)
                return Unauthorized();

            appointment.Status = AppointmentStatus.Completed;
            await _context.SaveChangesAsync();

            return Ok("Appointment completed.");
        }
    }
}