using System.ComponentModel.DataAnnotations;

namespace Smart.Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string UserName { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? RfidCardId { get; set; }

        public virtual RfidCard RfidCard { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
