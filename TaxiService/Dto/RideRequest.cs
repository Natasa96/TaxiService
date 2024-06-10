using System;
using DataLib.Model;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;

public class RideRequest
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public AddressDto StartAddress { get; set; }
    public AddressDto EndAddress { get; set; }
    public DateTime? EstimatedArrivalTime { get; set; }
    public DateTime? EstimatedRideTime { get; set; }
    public long RidePrice { get; set; }

    public DateTime? AcceptionTime { get; set; }
    public DateTime? FinishedTime { get; set; }
    public int? RideId { get; set; }
    public RideStatus? Status { get; set; }
}

public class AddressDto
{
    public string StreetName { get; set; }
    public string StreetNumber { get; set; }
    public string City { get; set; }
    public int ZipCode { get; set; }
}