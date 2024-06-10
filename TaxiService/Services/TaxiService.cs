using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Core;
using DataLib;
using DataLib.Model;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
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

    public async Task<RideResponse> GetRideByIdAsync(int id)
    {
        Ride ride = await _rideRepository.FindByIdAsync(id);
        return new RideResponse()
        {
            Id = ride.Id,
            Driver = ride?.Driver?.ToString() ?? "",
            User = ride?.User?.ToString() ?? "",
            Price = ride?.RidePrice ?? -1,
            StartAddress = ride?.StartAddress.ToString() ?? "",
            EndAddress = ride?.EndAddress.ToString() ?? "",
            EstimatedRideTime = ride?.EstimatedRideTime,
            EstimatedArrivalTime = ride.EstimatedArrivalTime,
            RideStatus = ride.Status
        };
    }

    public async Task<RideResponse> NewRide(RideRequest newRideRequest, int userId)
    {
        Ride returnRide;
        returnRide = await _rideRepository.AddAsync(new Ride()
        {
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
            UserId = userId,
            RidePrice = newRideRequest.RidePrice,
            EstimatedArrivalTime = newRideRequest.EstimatedArrivalTime,
            Status = 0
        });

        return new RideResponse()
        {
            Id = returnRide.Id,
            Driver = returnRide?.Driver?.ToString() ?? "",
            User = returnRide?.User?.ToString() ?? "",
            Price = returnRide?.RidePrice ?? -1,
            StartAddress = returnRide?.StartAddress.ToString() ?? "",
            EndAddress = returnRide?.EndAddress.ToString() ?? "",
            EstimatedRideTime = returnRide?.EstimatedRideTime,
            EstimatedArrivalTime = returnRide.EstimatedArrivalTime,
            RideStatus = returnRide.Status
        };
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
    public async Task<IEnumerable<RideResponse>> GetAllRides(Roles role, int userId)
    {
        try
        {
            IEnumerable<Ride> data;

            switch (role)
            {
                case Roles.Driver:
                    data = await _rideRepository.GetDriverAvailableRidesAsync();
                    break;
                case Roles.User:
                    data = await _rideRepository.GetUserRidesAsync(userId);
                    break;
                case Roles.Admin:
                default:
                    data = await _rideRepository.GetAllAsync();
                    break;
            }
            var returnRides = new List<RideResponse>() { };
            foreach (var ride in data)
            {
                var newRideResponce = new RideResponse()
                {
                    Id = ride.Id,
                    Price = ride.RidePrice,
                    Driver = ride?.Driver?.ToString() ?? "",
                    User = ride?.User?.ToString() ?? "",
                    StartAddress = ride?.StartAddress.ToString() ?? "",
                    EndAddress = ride?.EndAddress.ToString() ?? "",
                    EstimatedRideTime = ride?.EstimatedRideTime,
                    EstimatedArrivalTime = ride.EstimatedArrivalTime,
                    RideStatus = ride.Status
                };
                returnRides.Add(newRideResponce);
            }
            return returnRides;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<RideResponse>> GetHistoryRides(Roles role, int userId)
    {
        try
        {
            IEnumerable<Ride> data;

            switch (role)
            {
                case Roles.Driver:
                    data = await _rideRepository.GetDriverRidesAsync(userId);
                    break;
                case Roles.User:
                    data = await _rideRepository.GetUserRidesAsync(userId);
                    break;
                case Roles.Admin:
                default:
                    data = await _rideRepository.GetAllAsync();
                    break;
            }
            var returnRides = new List<RideResponse>() { };
            foreach (var ride in data)
            {
                var newRideResponce = new RideResponse()
                {
                    Id = ride.Id,
                    Price = ride.RidePrice,
                    Driver = ride?.Driver?.ToString() ?? "",
                    User = ride?.User?.ToString() ?? "",
                    StartAddress = ride?.StartAddress.ToString() ?? "",
                    EndAddress = ride?.EndAddress.ToString() ?? "",
                    EstimatedRideTime = ride?.EstimatedRideTime,
                    EstimatedArrivalTime = ride.EstimatedArrivalTime,
                    RideStatus = ride.Status
                };
                returnRides.Add(newRideResponce);
            }
            return returnRides;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        // throw new Exception("List of rides not found!");
    }
    public async Task<Ride> UpdateRide(UpdateRideRequest request, int driverId)
    {
        var ride = await _rideRepository.FindByIdAsync((int)request.RideId);

        switch (request.Status)
        {
            case RideStatus.Accepted:
                ride.AcceptedTime = request.AcceptionTime;
                ride.DriverId = driverId;
                ride.Status = RideStatus.Accepted;
                break;
            case RideStatus.InProgress:
                ride.StartTime = request.StartTime;
                ride.EstimatedRideTime = request.RideDuration;
                ride.Status = RideStatus.InProgress;
                break;
            case RideStatus.Finished:
                ride.EndTime = DateTime.Now;
                ride.Status = RideStatus.Finished;
                break;
            default:
                break;
        }
        await _rideRepository.UpdateAsync(ride);
        return ride;
    }

    public async Task<bool> CanDriverAcceptRides(UpdateRideRequest request, int driverId)
    {
        var driver = await _userRepository.GetByIdAsync(driverId);
        var ride = await _rideRepository.FindByIdAsync(request.RideId);
        if (driver.VerificationState != VerificationState.Approved)
            throw new Exception("Driver is not verified!");

        var rides = await _rideRepository.GetDriverRidesAsync(driverId);
        if (ride.Driver == null)
            return true;
        else
            throw new Exception("Driver already has an active ride!");

        return true;
    }

    public async Task<bool> CanDriverStartRides(UpdateRideRequest request, int driverId)
    {
        var driver = await _userRepository.GetByIdAsync(driverId);
        var ride = await _rideRepository.FindByIdAsync(request.RideId);
        if (driver.VerificationState != VerificationState.Approved)
            throw new Exception("Driver is not verified!");

        var rides = await _rideRepository.GetDriverRidesAsync(driverId);
        if (ride.Driver.Id == driver.Id)
            return true;
        else
            throw new Exception("Driver already has an active ride!");

        return true;
    }

    public async Task<bool> CanDriverFinishRides(UpdateRideRequest request, int driverId)
    {
        var driver = await _userRepository.GetByIdAsync(driverId);
        var ride = await _rideRepository.FindByIdAsync(request.RideId); ;

        if (driver.VerificationState != VerificationState.Approved)
            throw new Exception("Driver is not verified!");
        if (driver.Id == ride.Driver.Id)
        {
            if (ride.Status == RideStatus.InProgress)
            {
                return true;
            }
            else;
            {
                throw new Exception("Cannot finish ride");
            }
        }

        return true;
    }
}