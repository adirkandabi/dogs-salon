namespace DogsSalon.Models
{
    public class DogSize
    {
        public int Id { get; set; }
        public required string SizeName { get; set; } // "Small", "Medium", "Large"
        public int DurationMinutes { get; set; }  
        public decimal BasePrice { get; set; } 
    }
}
