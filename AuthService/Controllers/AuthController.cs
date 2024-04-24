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
  [HttpPost("register")]
  public IActionResult Register(RegisterRequest request)
  {
    return Ok(request);
  }


}