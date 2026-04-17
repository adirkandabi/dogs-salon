namespace DogsSalon.DTOs
{
    public record CreateAppointmentDto(int DogSizeId, DateTime AppointmentDate);

    public record AppointmentSummaryDto(
        int AppointmentId,
        int UserId,
        string CustomerName,
        string DogSizeName,
        DateTime AppointmentDate,
        decimal Price,
        DateTime CreatedAt
    );
    public record DogSizeDto(int Id,string SizeName, int DurationMinutes, decimal BasePrice);
}
