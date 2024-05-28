using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using BCrypt.Net;
using DataLib;
using DataLib.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ZstdSharp.Unsafe;


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

    if (user.Role == Roles.Driver)
      if (user.VerificationState != VerificationState.Approved)
        throw new Exception("Driver not verified!");


    var isPasswordValid = VerifyPassword(loginRequest.Password, user.Password);

    if (!isPasswordValid)
      throw new Exception("Password is incorect!");

    var jwtToken = GenerateToken(Convert.ToString(user.Id), user.Role.ToString());

    return jwtToken;
  }


  public async Task<RegisterResponse> UpdateUser(RegisterRequest user, int userId)
  {
    //update user logic here
    var userUpdate = await _repository.GetByIdAsync(userId);
    userUpdate.Name = user.Name;
    userUpdate.Surname = user.Surname;
    userUpdate.Password = HashPassword(user.Password);
    userUpdate.Birthday = user.Birthday;
    userUpdate.Picture = user.Picture;
    userUpdate.Address = new Address()
    {
      StreetName = user.Address.StreetName,
      StreetNumber = user.Address.StreetNumber,
      City = user.Address.City,
      ZipCode = user.Address.ZipCode
    };

    await _repository.UpdateAsync(userUpdate);
    return new RegisterResponse() { Message = "User is updated successfully" };
  }

  public async void VerifyDriver(int idDriver)
  {
    var driver = await _repository.GetByIdAsync(idDriver);
    if (driver.VerificationState == VerificationState.Approved)
      throw new Exception("Driver state is already approved!");

    driver.VerificationState = VerificationState.Approved;
    await _repository.UpdateAsync(driver);
  }

  public async Task<User> GetUser(string token)
  {
    var tokenData = DecryptToken(token);
    tokenData.TryGetValue("UserId", out var userId);
    Console.WriteLine(tokenData);

    return await _repository.GetByIdAsync(Convert.ToInt32(userId));
  }

  private string HashPassword(string password)
  {
    return BCrypt.Net.BCrypt.HashPassword(password);
  }
  private bool VerifyPassword(string password, string hashedPassword)
  {
    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
  }

  private string GenerateToken(string userId, string userRole)
  {
    string secretKey = "wdFjiHj0P+ogzJ3BQb0W9Yd2MBj8DVhO3TkH5qtd3Eo=";
    TimeSpan tokenLifetime = TimeSpan.FromMinutes(30);

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Convert.FromBase64String(secretKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[]
            {
                new Claim("UserId", userId),
                new Claim(ClaimTypes.Role, userRole)
            }),
      Issuer = "TaxiService",
      Audience = "TaxiService",
      Expires = DateTime.UtcNow.Add(tokenLifetime),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }

  private Dictionary<string, string> DecryptToken(string encryptedToken)
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

    return token.Claims.ToDictionary(c => c.Type, c => c.Value);
  }


}