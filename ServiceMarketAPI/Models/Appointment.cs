using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ServiceMarketAPI.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        [ForeignKey("CustomerId")]
        public ApplicationUser Customer { get; set; } = null!;



        public int ServiceListingId { get; set; }
        [ForeignKey("ServiceListingId")]
        public ServiceListing ServiceListing { get; set; } = null!;

        public DateTime Date { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    }
}