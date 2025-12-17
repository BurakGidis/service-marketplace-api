using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ServiceMarketAPI.DTOs.Appointments
{
    public class CreateAppointmentRequest
    {
        [Required]
        public int ServiceListingId { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}