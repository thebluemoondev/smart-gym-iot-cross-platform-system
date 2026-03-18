using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        public string? RfidCardGuid { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public string Role { get; set; } = "Member";

        public ICollection<Subscription>? Subscriptions { get; set; }
    }
}