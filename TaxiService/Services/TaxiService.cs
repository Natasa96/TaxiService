using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using DataLib;
using DataLib.Model;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;

public class TaxiService
{
  private readonly IRideRepository _rideRepository;
  private readonly IUserRepository _userRepository;
  public TaxiService(IRideRepository rideRepository, IUserRepository userRepository)
  {
    _rideRepository = rideRepository;
    _userRepository = userRepository;
  }

  public async Task<RideResponse> NewRide(RideRequest newRideRequest)
  {
    await _rideRepository.AddAsync(new Ride()
    {
      StartTime = newRideRequest.StartTime,
      EndTime = newRideRequest.EndTime,
      StartAddress = new Address
      {
        StreetName = newRideRequest.StartAddress.StreetName,
        StreetNumber = newRideRequest.StartAddress.StreetNumber,
        ZipCode = newRideRequest.StartAddress.ZipCode,
        City = newRideRequest.StartAddress.City
      },
      EndAddress = new Address
      {
        StreetName = newRideRequest.EndAddress.StreetName,
        StreetNumber = newRideRequest.EndAddress.StreetNumber,
        City = newRideRequest.EndAddress.City,
        ZipCode = newRideRequest.EndAddress.ZipCode
      },
      UserId = newRideRequest.UserId,
      DriverId = newRideRequest.DriverId
    });

    return new RideResponse() { Message = "New RIDE created successfully!" };
  }

  public async Task<IEnumerable<User>> GetAllDrivers()
  {
    var drivers = await _userRepository.GetAllAsync();

    return drivers.Where<User>(d => d.Role == Roles.Driver);
  }

  /* Getting:
      ADMIN: all past rides
      DRIVER: all past rides of driver with userId
      USER: all his past rides
  */
  public async Task<IEnumerable<Ride>> GetAllRides(Roles role, int userId)
  {
    //logic
    switch (role)
    {
      case Roles.Driver:
        await _rideRepository.GetByRoleAsync(role, userId);
        break;
      case Roles.User:
        await _rideRepository.GetByRoleAsync(role, userId);
        break;
      default:
        await _rideRepository.GetAllAsync();
        break;
    }
    throw new Exception("List of rides not found!");
  }
  public async Task<Ride> UpdateRide()
  {
    //logic
    // can do switch logic based on some variable to accept, start and finish a ride
    throw new NotImplementedException();
  }
}