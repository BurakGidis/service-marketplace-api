using System.ComponentModel.DataAnnotations;

namespace ServiceMarketAPI.DTOs.Services
{
    public class UpdateServiceRequest
    {
        [Required]
        public string Title { get; set; }=string.Empty;

        [Required]
        public string Description { get; set; }=string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string City { get; set; }=string.Empty;
        
        
        public int CategoryId { get; set; }
    }
}