namespace api.Models
{
    public class BodyIndex
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double BMI => Weight / ((Height / 100) * (Height / 100));
        public string Note { get; set; } = string.Empty;
        public DateTime RecordedDate { get; set; } = DateTime.Now;

        public Users? User { get; set; }
    }
}
