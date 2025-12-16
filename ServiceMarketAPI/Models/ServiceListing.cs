using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceMarketAPI.Models
{
    public class ServiceListing
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [MaxLength(100)]
        public string Title { get; set; }=string.Empty; 

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }=string.Empty; 

        [Range(0, 100000)]
        public decimal Price { get; set; }

        [Required]
        public string City { get; set; }=string.Empty; 

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string UserId { get; set; }=string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }= null!;
    }
}