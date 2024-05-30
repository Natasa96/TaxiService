using System.Security.Claims;
using DataLib.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/taxi")]

/**
TODO:
 Start microservices. See what to use for communication between microservices
 Pomeri dovlacenje korisnika iz TaxiService u AuthService

 Napraviti novi EF model za ocenjivanje vozaca koji ce imati userId, driverId i ocenu
 Dodati novi status verifikacije za vozace --BLOCKED--

 Popraviti vreme cekanja korisnika (da ne zavisi od vozaca), popraviti vreme trajanja voznje iz minuti ---> trenutni datum + min 
 i dodati cenu voznje 
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
  public async Task<IActionResult> AcceptRide(RideUpdateRequest request)
  {
    try
    {
      var driverId = User.Claims.FirstOrDefault(i => i.Type == "UserId").Value;
      bool valid = await _taxiService.CanDriverAcceptRides(request, Convert.ToInt32(driverId));

      if (valid)
        await _taxiService.UpdateRide(request, Convert.ToInt32(driverId));

      return Ok();
    }
    catch (Exception ex) { return BadRequest(ex.Message); }

  }

  [Authorize(Policy = "DriverOnly")]
  [HttpPost("start-ride")]
  public async Task<IActionResult> StartRide(RideUpdateRequest request)
  {
    try
    {
      var driverId = User.Claims.FirstOrDefault(i => i.Type == "UserId").Value;
      bool valid = await _taxiService.CanDriverAcceptRides(request, Convert.ToInt32(driverId));

      if (valid)
        await _taxiService.UpdateRide(request, Convert.ToInt32(driverId));

      return Ok();
    }
    catch (Exception ex) { return BadRequest(ex.Message); }
  }

  [Authorize(Policy = "DriverOnly")]
  [HttpPost("finish-ride")]
  public async Task<IActionResult> FinishRide(RideUpdateRequest request)
  {
    try
    {
      var driverId = User.Claims.FirstOrDefault(i => i.Type == "UserId").Value;
      bool validAccept = await _taxiService.CanDriverAcceptRides(request, Convert.ToInt32(driverId));
      bool validFinish = await _taxiService.CanDriverFinishRides(request, Convert.ToInt32(driverId));

      if (validAccept && validFinish)
        await _taxiService.UpdateRide(request, Convert.ToInt32(driverId));

      return Ok();
    }
    catch (Exception ex) { return BadRequest(ex.Message); }
  }

}