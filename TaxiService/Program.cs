using System.Configuration;
using System.Text;
using DataLib;
using DataLib.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Bcpg;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TaxiDbContext>(options =>
options.UseMySQL("Server=mysql,3306;Database=taxiservicedb;User Id=admin;Password=admin;"));
builder.Services.AddScoped<IRideRepository, RideRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<TaxiService>();
builder.Services.AddControllers();

//Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = "TaxiService",
      ValidAudience = "TaxiService",
      IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String("wdFjiHj0P+ogzJ3BQb0W9Yd2MBj8DVhO3TkH5qtd3Eo="))
    };
  });

builder.Services.AddAuthorizationBuilder()
  .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
  .AddPolicy("DriverOnly", policy => policy.RequireRole(Roles.Driver.ToString()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  // app.UseSwagger();
  // app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
  endpoints.MapControllers();
});

app.MapControllers();

app.Run();
