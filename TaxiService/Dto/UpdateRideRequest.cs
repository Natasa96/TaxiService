using DataLib.Model;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using System.Text.Json.Serialization;


public class UpdateRideRequest
{
    public int RideId { get; set; }
    public DateTime? AcceptionTime { get; set; }
    public DateTime? StartTime { get; set; }
    [JsonPropertyName("rideDuration")]
    public DateTime? RideDuration { get; set; }
    public DateTime? FinishedTime { get; set; }
    public RideStatus Status { get; set; }

}