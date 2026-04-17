using DogsSalon.Data;
using DogsSalon.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using DogsSalon.Models;

namespace DogsSalon.Services;

public class AppointmentService
{
    private readonly ApplicationDbContext _context;

    public AppointmentService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<DogSizeDto>> GetDogSizes()
    {
                return await _context.DogSizes
            .Select(ds => new DogSizeDto(ds.Id, ds.SizeName, ds.DurationMinutes, ds.BasePrice))
            .ToListAsync();
    }

    // Create appointment and calculate discount using stored procedure
    public async Task CreateAppointmentAsync(int userId, CreateAppointmentDto dto)
    {
        try
        {
            var parameters = new[] {
            new SqlParameter("@UserId", userId),
            new SqlParameter("@DogSizeId", dto.DogSizeId),
            new SqlParameter("@AppointmentDate", dto.AppointmentDate)
        };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_CreateAppointment @UserId, @DogSizeId, @AppointmentDate", parameters);
        }
        catch (SqlException ex) when (ex.Number == 50000 || ex.Message.Contains("Conflict"))
        {
            
            throw new InvalidOperationException(ex.Message);
        }
    }

    // Retrieve all appointments through the view
    public async Task<List<AppointmentSummaryDto>> GetAllSummariesAsync()
    {
        return await _context.AppointmentSummaries
            .Select(v => new AppointmentSummaryDto(
                v.AppointmentId,
                v.UserId,
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
        try
        {
            var parameters = new[] {
            new SqlParameter("@AppointmentId", appointmentId),
            new SqlParameter("@UserId", userId),
            new SqlParameter("@DogSizeId", dto.DogSizeId),
            new SqlParameter("@AppointmentDate", dto.AppointmentDate)
        };

            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateAppointment @AppointmentId, @UserId, @DogSizeId, @AppointmentDate", parameters);

            return rowsAffected > 0;
        }
        catch (SqlException ex) when (ex.Number == 50000 || ex.Message.Contains("Conflict"))
        {
            throw new InvalidOperationException(ex.Message);
        }
    }
}