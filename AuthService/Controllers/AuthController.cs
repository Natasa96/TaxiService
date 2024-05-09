using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text.Json;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private AuthService _authService;
  public AuthController(AuthService authService)
  {
    _authService = authService;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterRequest request)
  {
    var response = await _authService.Register(request);
    return Ok(response);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginRequest request)
  {
    var token = await _authService.Login(request);
    return Ok(token);
  }

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
}