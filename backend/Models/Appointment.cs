namespace DogsSalon.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int DogSizeId { get; set; }
        public DogSize? DogSize { get; set; }

        public DateTime AppointmentDate { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Final price after applying any discounts or promotions
        public decimal FinalPrice { get; set; } 
    }
}
