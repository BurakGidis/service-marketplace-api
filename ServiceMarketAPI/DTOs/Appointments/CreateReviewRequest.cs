using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ServiceMarketAPI.DTOs.Appointments
{
    public class CreateReviewRequest
    {
        [Required]
        public int AppointmentId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}