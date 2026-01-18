using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Smart.Api.Models
{
    public class RfidCard
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CardUid { get; set; }
        public bool IsActive { get; set; } = true;

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }


    }
}
