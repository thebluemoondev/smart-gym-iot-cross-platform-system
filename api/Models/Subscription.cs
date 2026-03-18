using api.Models;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public Users? User { get; set; }

        public int GymPackageId { get; set; }
        public GymPackage? GymPackage { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}