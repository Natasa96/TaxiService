using DataLib.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TaxiDbContext>(options =>
options.UseMySQL("Server=mysql,3306;Database=taxiservicedb;User Id=admin;Password=admin;"));        //<------------- TODO
builder.Services.AddScoped<IRideRepository, RideRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<TaxiService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddSingleton<RabbitMQClient>(sp =>
    new RabbitMQClient("taxirabbitmq", "chatQueue"));
builder.Services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddAuthorizationBuilder()
  .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
  .AddPolicy("UserOnly", policy => policy.RequireRole(Roles.User.ToString()))
  .AddPolicy("DriverOnly", policy => policy.RequireRole(Roles.Driver.ToString()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseCors("AllowAll");

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
