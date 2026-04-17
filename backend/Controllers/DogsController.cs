using DogsSalon.DTOs;
using DogsSalon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogsSalon.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires authentication for all endpoints in this controller
public class DogsController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public DogsController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

   

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dogs = await _appointmentService.GetDogSizes();
        return Ok(dogs);
    }

}