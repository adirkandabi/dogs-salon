using DogsSalon.Data;
using DogsSalon.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace DogsSalon.Services;

public class AppointmentService
{
    private readonly ApplicationDbContext _context;

    public AppointmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Create appointment and calculate discount using stored procedure
    public async Task CreateAppointmentAsync(int userId, CreateAppointmentDto dto)
    {
        var userIdParam = new SqlParameter("@UserId", userId);
        var dogSizeIdParam = new SqlParameter("@DogSizeId", dto.DogSizeId);
        var dateParam = new SqlParameter("@AppointmentDate", dto.AppointmentDate);

        // Execute the stored procedure to create the appointment and calculate the discount
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_CreateAppointment @UserId, @DogSizeId, @AppointmentDate",
            userIdParam, dogSizeIdParam, dateParam);
    }

    // Retrieve all appointments through the view
    public async Task<List<AppointmentSummaryDto>> GetAllSummariesAsync()
    {
        return await _context.AppointmentSummaries
            .Select(v => new AppointmentSummaryDto(
                v.AppointmentId,
                v.CustomerName,
                v.DogSizeName,
                v.AppointmentDate,
                v.Price,
                v.CreatedAt
            ))
            .ToListAsync();
    }
    // Delete appointment
    public async Task<bool> DeleteAppointmentAsync(int appointmentId, int userId)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.UserId == userId);

        if (appointment == null) return false; // The appointment does not exist or does not belong to the user

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }

    // Update appointment 
    public async Task<bool> UpdateAppointmentAsync(int appointmentId, int userId, CreateAppointmentDto dto)
    {
        var idParam = new SqlParameter("@AppointmentId", appointmentId);
        var userParam = new SqlParameter("@UserId", userId);
        var sizeParam = new SqlParameter("@DogSizeId", dto.DogSizeId);
        var dateParam = new SqlParameter("@AppointmentDate", dto.AppointmentDate);

        
        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_UpdateAppointment @AppointmentId, @UserId, @DogSizeId, @AppointmentDate",
            idParam, userParam, sizeParam, dateParam);

        return rowsAffected > 0;
    }
}