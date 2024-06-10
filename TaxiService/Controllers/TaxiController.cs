using System;
using System.Reflection;
using System.Security.Claims;
using DataLib.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/taxi")]

/**
TODO:
 Start microservices. See what to use for communication between microservices
 Pomeri dovlacenje korisnika iz TaxiService u AuthService

 Popraviti vreme cekanja korisnika (da ne zavisi od vozaca)
*/
public class TaxiController : ControllerBase
{
    private readonly TaxiService _taxiService;

    private readonly RabbitMQClient _rabbitMQClient;

    public TaxiController(TaxiService taxiService, RabbitMQClient rabbitMQClient)
    {
        _taxiService = taxiService;
        _rabbitMQClient = rabbitMQClient;
    }

    [Authorize(Policy = "UserOnly")]
    [HttpPost("create")]      //creating new ride
    public async Task<IActionResult> Create(RideRequest request)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(i => i.Type == "UserId").Value;
            var responce = await _taxiService.NewRide(request, Int32.Parse(userId));
            return Ok(responce);
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [Authorize]
    [HttpGet("rides")]
    public async Task<IActionResult> GetRides()
    {
        try
        {
            var userRole = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            Roles role = (Roles)Enum.Parse(typeof(Roles), userRole);

            var userId = User.Claims.FirstOrDefault(i => i.Type == "UserId").Value;
            var getRides = await _taxiService.GetAllRides(role, Convert.ToInt32(userId));
            return Ok(getRides);
        }
        catch (Exception ex) { return BadRequest(ex.Message); }

    }
    [Authorize]
    [HttpGet("history")]
    public async Task<IActionResult> GetHistoryRides()
    {
        try
        {
            var userRole = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            Roles role = (Roles)Enum.Parse(typeof(Roles), userRole);

            var userId = User.Claims.FirstOrDefault(i => i.Type == "UserId").Value;
            var getRides = await _taxiService.GetHistoryRides(role, Convert.ToInt32(userId));
            return Ok(getRides);
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [Authorize]
    [HttpGet("rides/{id}")]
    public async Task<IActionResult> GetRideById(int id)
    {
        try
        {
            var getRides = await _taxiService.GetRideByIdAsync(id);
            return Ok(getRides);
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [Authorize(Policy = "DriverOnly")]
    [HttpPost("accept-ride")]
    public async Task<IActionResult> AcceptRide(UpdateRideRequest request)
    {
        try
        {
            var driverId = User.Claims.FirstOrDefault(i => i.Type == "UserId").Value;
            bool valid = await _taxiService.CanDriverAcceptRides(request, Convert.ToInt32(driverId));
            if (!valid)
            {
                return BadRequest("Cannot accept a new ride");
            }
            Ride ride = await _taxiService.UpdateRide(request, Convert.ToInt32(driverId)); ;
            return Ok(new RideResponse()
            {
                Id = ride.Id,
                Driver = ride?.Driver.ToString(),
                Price = ride.RidePrice,
                EndAddress = ride?.EndAddress.ToString(),
                StartAddress = ride?.StartAddress.ToString(),
                User = ride?.User.ToString(),
                EstimatedRideTime = ride?.EstimatedRideTime,
                EstimatedArrivalTime = ride?.EstimatedArrivalTime,
                RideStatus = ride.Status
            });
        }
        catch (Exception ex) { return BadRequest(ex.Message); }

    }

    [Authorize(Policy = "DriverOnly")]
    [HttpPut("start-ride")]
    public async Task<IActionResult> StartRide(UpdateRideRequest request)
    {
        try
        {
            var driverId = User.Claims.FirstOrDefault(i => i.Type == "UserId").Value;
            bool valid = await _taxiService.CanDriverStartRides(request, Convert.ToInt32(driverId));

            if (!valid)
            {
                return BadRequest("Driver is not verified");
            }

            await _taxiService.UpdateRide(request, Convert.ToInt32(driverId));

            var ride = await _taxiService.GetRideByIdAsync((int)request.RideId);
            return Ok(ride);
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [Authorize(Policy = "DriverOnly")]
    [HttpPut("finish-ride")]
    public async Task<IActionResult> FinishRide(UpdateRideRequest request)
    {
        try
        {
            var driverId = User.Claims.FirstOrDefault(i => i.Type == "UserId").Value;
            bool validFinish = await _taxiService.CanDriverFinishRides(request, Convert.ToInt32(driverId));

            if (!validFinish)
            {
                return BadRequest("cannot finish ride");
            }

            var ride = await _taxiService.UpdateRide(request, Convert.ToInt32(driverId));

            return Ok(new RideResponse()
            {
                Id = ride.Id,
                Driver = ride?.Driver.ToString(),
                Price = ride.RidePrice,
                EndAddress = ride?.EndAddress.ToString(),
                StartAddress = ride?.StartAddress.ToString(),
                User = ride?.User.ToString(),
                EstimatedRideTime = ride?.EstimatedRideTime,
                EstimatedArrivalTime = ride?.EstimatedArrivalTime,
                RideStatus = ride.Status
            });
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

}