using System.Security.Claims;
using DataLib.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/taxi")]

/**
TODO:
Create a route to update profile (PUT /profile) (user can only update his own profile (hint: extract userID from jwt token))
Create new ride (POST /ride). Check if the ride with user id and driver id exists throw error
Get rides(GET /ride). Based on JWT token role return following(Admin -> all rides, Driver -> Rides where driver id is null (new ride))
 and rides where driverID matches drivers id (past rides), User -> rides where userID=users id and driverID is not null (past rides))

 Start microservices. See what to use for communication between microservices
*/
public class TaxiController : ControllerBase
{
  private readonly TaxiService _taxiService;
  public TaxiController(TaxiService taxiService)
  {
    _taxiService = taxiService;
  }

  [Authorize(Policy = "UserOnly")]
  [HttpPost("create")]      //creating new ride
  public async Task<IActionResult> Create(RideRequest request)
  {
    try
    {
      var responce = await _taxiService.NewRide(request);
      Console.WriteLine(responce.ToString());
      return Ok(responce);
    }
    catch (Exception ex) { return BadRequest(ex.Message); }
  }

  [Authorize(Policy = "AdminOnly")]
  [HttpGet("drivers")]
  public IActionResult GetDrivers()
  {
    var drivers = _taxiService.GetAllDrivers();

    return Ok(drivers);
  }

  [Authorize]
  [HttpGet("rides")]
  public async Task<IActionResult> GetRides()
  {
    try
    {
      // get rides based on jwt token data
      var userRole = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;
      Roles role = (Roles)Enum.Parse(typeof(Roles), userRole);

      var userId = User.Claims.FirstOrDefault(i => i.Type == "UserId").Value;
      var getRides = await _taxiService.GetAllRides(role, Convert.ToInt32(userId));
      return Ok(getRides);
    }
    catch (Exception ex) { return BadRequest(ex.Message); }
  }


  [Authorize(Policy = "DriverOnly")]
  [HttpPost("accept-ride")]
  public IActionResult AcceptRide(RideUpdateRequest request)
  {
    //assign driver to the ride
    return Ok();
  }

  [Authorize(Policy = "DriverOnly")]
  [HttpPost("start-ride")]
  public IActionResult StartRide(RideUpdateRequest request)
  {
    // start ride
    // returns estimation time when taxi will come to the user
    // when ride is started user cannot request new rides
    // driver cannot accept new rides
    return Ok();
  }

  [Authorize(Policy = "DriverOnly")]
  [HttpPost("finish-ride")]
  public IActionResult FinishRide(RideUpdateRequest request)
  {
    // finish ride
    // after this action the user can request new rides
    return Ok();
  }

}