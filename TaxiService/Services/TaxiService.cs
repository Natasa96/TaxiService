using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Azure.Core;
using DataLib;
using DataLib.Model;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
      DRIVER: all past rides of driver with userId(driverId)
      USER: all his past rides
  */
  public async Task<IEnumerable<Ride>> GetAllRides(Roles role, int userId)
  {
    switch (role)
    {
      case Roles.Driver:
        await _rideRepository.GetDriverRidesAsync(userId);
        break;
      case Roles.User:
        await _rideRepository.GetUserRidesAsync(userId);
        break;
      default:
        await _rideRepository.GetAllAsync();
        break;
    }
    throw new Exception("List of rides not found!");
  }
  public async Task<Ride> UpdateRide(RideUpdateRequest request, int driverId)
  {
    var ride = await _rideRepository.FindByIdAsync(request.RideId);
    Random estimatedTime = new Random();

    switch (request.status)
    {
      case RideStatus.Available:
        ride.AcceptedTime = request.AcceptionTime;
        ride.DriverId = driverId;
        break;
      case RideStatus.InProgress:
        ride.StartTime = request.StartTime;
        ride.EstimatedRideTime = (uint)estimatedTime.Next(1, 10);
        break;
      case RideStatus.Finished:
        ride.EndTime = request.FinishedTime;
        break;
      default:                            //Canceled ride
        ride.EndTime = DateTime.Now;
        break;
    }
    await _rideRepository.UpdateAsync(ride);

    throw new NotImplementedException();
  }

  public async Task<bool> CanDriverAcceptRides(RideUpdateRequest request, int driverId)
  {
    var driver = await _userRepository.GetByIdAsync(driverId);
    if (driver.VerificationState != VerificationState.Approved)
      throw new Exception("Driver is not verified!");

    var rides = await _rideRepository.GetDriverRidesAsync(driverId);
    if (rides.Select(r => r.EndTime == null).Count() > 0)
      throw new Exception("Driver already has an active ride!");

    return true;
  }

  public async Task<bool> CanDriverFinishRides(RideUpdateRequest request, int driverId)
  {
    var rides = await _rideRepository.GetDriverRidesAsync(driverId);

    if (rides.Select(r => r.StartTime == null).Count() > 0)
      throw new Exception("This ride is not started!");

    return true;
  }
}