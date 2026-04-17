namespace DogsSalon.Models
{
    public class AppointmentSummaryView
    {
        public int AppointmentId { get; set; }
        public int UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? DogSizeName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
