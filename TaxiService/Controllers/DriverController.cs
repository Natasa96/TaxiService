
using System.Security.Claims;
using DataLib.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/taxi")]

public class DriverController : ControllerBase
{
    private readonly DriverService _driverService;
    public DriverController(DriverService driverService)
    {
        _driverService = driverService;
    }
    [Authorize(Policy = "AdminOnly")]
    [HttpGet("drivers")]
    public async Task<IActionResult> GetDrivers()
    {
        var drivers = await _driverService.GetAllDrivers();
        return Ok(drivers);
    }

}