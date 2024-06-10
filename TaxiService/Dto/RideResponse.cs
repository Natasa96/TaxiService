
using System;
using DataLib.Model;

public class RideResponse
{
    public int Id { get; set; }
    public string StartAddress { get; set; }
    public string EndAddress { get; set; }
    public DateTime? EstimatedRideTime { get; set; }
    public DateTime? EstimatedArrivalTime { get; set; }
    public string? Driver { get; set; }
    public string? User { get; set; }
    public long Price { get; set; }

    public RideStatus RideStatus { get; set; }
}