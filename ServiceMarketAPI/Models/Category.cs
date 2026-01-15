using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServiceMarketAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } =string.Empty;

      [JsonIgnore]
        public ICollection<ServiceListing> ServiceListings { get; set; }= null!;
    }
}