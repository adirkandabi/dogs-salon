using DogsSalon.DTOs;
using DogsSalon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogsSalon.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires authentication for all endpoints in this controller
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public AppointmentsController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    // Fetches the user ID from the JWT token claims.
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var summaries = await _appointmentService.GetAllSummariesAsync();
        return Ok(summaries);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            await _appointmentService.CreateAppointmentAsync(GetUserId(), dto);
            return Ok(new { message = "Appointment created successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var success = await _appointmentService.UpdateAppointmentAsync(id, GetUserId(), dto);
            if (!success) return NotFound(new { message = "Appointment not found" });
            return Ok(new { message = "Updated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _appointmentService.DeleteAppointmentAsync(id, GetUserId());
        if (!success) return NotFound(new { message = "Appointment not found or not authorized" });

        return Ok(new { message = "Appointment deleted successfully" });
    }
}