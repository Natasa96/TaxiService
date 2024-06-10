using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;
using DataLib.Model;
using Microsoft.IdentityModel.Tokens;

//TODO: Ocenjivanje (UserOnly)
//TODO: Implementirati logovanje pomocu drustvenih mreza!
public class AuthService
{
    private IUserRepository _repository;
    private RabbitMQClient _rabbitmqClient;
    public AuthService(IUserRepository repository, RabbitMQClient rabbitMQClient)
    {
        _repository = repository;
        _rabbitmqClient = rabbitMQClient;
    }

    public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
    {
        registerRequest.Password = HashPassword(registerRequest.Password);

        await _repository.AddAsync(new User()
        {
            Username = registerRequest.Username,
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
        var user = await _repository.FindByUsernameAsync(loginRequest.Username);

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
        var userUpdate = await _repository.GetByIdAsync(userId);

        if (user.Name != String.Empty)
            userUpdate.Name = user.Name;

        if (user.Surname != String.Empty)
            userUpdate.Surname = user.Surname;

        if (user.Password != string.Empty)
            userUpdate.Password = HashPassword(user.Password);

        userUpdate.Birthday = user.Birthday;

        if (user.Picture != null || user.Picture != String.Empty)
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

    public async Task<DriverStatusResponse> VerifyDriver(int idDriver)
    {
        var driver = await _repository.GetByIdAsync(idDriver);
        if (driver.VerificationState == VerificationState.Approved)
            throw new Exception("Driver state is already approved!");

        driver.VerificationState = VerificationState.Approved;
        await _repository.UpdateAsync(driver);
        _rabbitmqClient.SendMessage(new RabbitEmailRequest()
        {
            Email = driver.Email,
            DriverName = driver.Username,
        });
        return new DriverStatusResponse()
        {
            DriverId = idDriver,
            Status = VerificationState.Approved.ToString()
        };
    }

    //Blokiranje vozaca
    public async Task<DriverStatusResponse> BlockDriver(int driverId)
    {
        var driver = await _repository.GetByIdAsync(driverId);
        if (driver.VerificationState == VerificationState.Blocked)
            throw new Exception("Driver is already blocked!");

        driver.VerificationState = VerificationState.Blocked;
        await _repository.UpdateAsync(driver);
        return new DriverStatusResponse()
        {
            DriverId = driverId,
            Status = VerificationState.Blocked.ToString()
        };
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