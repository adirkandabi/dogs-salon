namespace DogsSalon.DTOs
{
    public record CreateAppointmentDto(int DogSizeId, DateTime AppointmentDate);

    public record AppointmentSummaryDto(
        int AppointmentId,
        string CustomerName,
        string DogSizeName,
        DateTime AppointmentDate,
        decimal Price,
        DateTime CreatedAt
    );
}
