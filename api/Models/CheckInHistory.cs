using api.Models;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class CheckInHistory
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public Users? User { get; set; }

        public DateTime CheckInTime { get; set; } = DateTime.Now;
        public bool AccessGranted { get; set; }
    }
}