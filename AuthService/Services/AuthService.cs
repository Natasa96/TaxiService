using DataLib;
using DataLib.Model;
using BCrypt.Net;
using System;
//using System.Exception;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;


public class AuthService
{
  private IUserRepository _repository;
  public AuthService(IUserRepository repository)
  {
    _repository = repository;
  }

  public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
  {
    registerRequest.Password = HashPassword(registerRequest.Password);

    await _repository.AddAsync(new User()
    {
      User_name = registerRequest.User_name,
      Password = registerRequest.Password,
      Name = registerRequest.Name,
      Surname = registerRequest.Surname,
      Email = registerRequest.Email,
      Birthday = registerRequest.Birthday,
      Address = new Address()
      {
        StreetName = registerRequest.Address.StreetName,
        StreetNumber = registerRequest.Address.StreetNumber,
        City = registerRequest.Address.City,
        ZipCode = registerRequest.Address.ZipCode
      },
      Picture = registerRequest.Picture,
      Role = (Roles)registerRequest.Role
    });

    return new RegisterResponse() { Message = "User created successufully!" };
  }

  public async Task<string> Login(LoginRequest loginRequest)
  {
    var user = await _repository.FindByUsernameAsync(loginRequest.Username);  //User izvucen iz baze
    if (user == null)
      throw new Exception("User does not exists!");

    var isPasswordValid = VerifyPassword(loginRequest.Password, user.Password);

    if (!isPasswordValid)
      throw new Exception("Password is incorect!");

    var jwtToken = GenerateToken(Convert.ToString(user.Id));

    return jwtToken;
  }

  public bool VerifyDriver(int idDriver)
  {
    //TODO <------------
    return true;
  }

  public async Task<User> GetUser(string token)
  {
    var tokenData = DecryptToken(token);
    Console.WriteLine(tokenData);

    return new User();
  }

  private string HashPassword(string password)
  {
    return BCrypt.Net.BCrypt.HashPassword(password);
  }
  private bool VerifyPassword(string password, string hashedPassword)
  {
    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
  }

  private string GenerateToken(string userId)
  {
    string secretKey = "wdFjiHj0P+ogzJ3BQb0W9Yd2MBj8DVhO3TkH5qtd3Eo=";
    TimeSpan tokenLifetime = TimeSpan.FromMinutes(30);

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Convert.FromBase64String(secretKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("Role", "Admin")
            }),
      Expires = DateTime.UtcNow.Add(tokenLifetime),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }

  private string DecryptToken(string encryptedToken)
  {
    var handler = new JwtSecurityTokenHandler();
    var token = handler.ReadToken(encryptedToken) as JwtSecurityToken;

    var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Convert.FromBase64String("wdFjiHj0P+ogzJ3BQb0W9Yd2MBj8DVhO3TkH5qtd3Eo="));

    var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
      IssuerSigningKey = securityKey,
      ValidateIssuer = false,
      ValidateAudience = false,
      ValidateLifetime = false
    };

    var claimsPrincipal = handler.ValidateToken(encryptedToken, tokenValidationParameters, out var validatedToken);

    return token.Claims.ToList();
  }
}