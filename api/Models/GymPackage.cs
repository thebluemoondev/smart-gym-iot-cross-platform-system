using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class GymPackage
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int DurationDays { get; set; }
    }
}