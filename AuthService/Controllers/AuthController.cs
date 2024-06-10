using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var response = await _authService.Register(request);
        Console.WriteLine(response.ToString());
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var token = await _authService.Login(request);
            return Ok(new { token = token });
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
            return BadRequest("Authorization header is missing.");

        var authHeader = authHeaderValue.ToString();
        var token = authHeader.Split(' ')[1];

        var user = await _authService.GetUser(token);
        return Ok(user);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyDriver(DriverStatusRequest request)
    {
        try
        {
            return Ok(await _authService.VerifyDriver(request.DriverId));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile(RegisterRequest request)
    {
        try
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "UserId").Value;
            var responce = await _authService.UpdateUser(request, Convert.ToInt32(userId));
            return Ok(responce);
        }
        catch (Exception ex) { return BadRequest(ex.Message); }

    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("block")]
    public async Task<IActionResult> BlockDriver(DriverStatusRequest request)
    {
        try
        {
            return Ok(await _authService.BlockDriver(request.DriverId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}