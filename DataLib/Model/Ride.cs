using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLib;

namespace DataLib.Model
{
    public class Ride
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }   //when Ride was created
        public DateTime? StartTime { get; set; }    //when Driver started the Ride
        public DateTime? EndTime { get; set; }
        public DateTime? AcceptedTime { get; set; } //when Ride was accepted
        [ForeignKey("StartAddress")]
        public int StartAddressId { get; set; }
        required public virtual Address StartAddress { get; set; }
        [ForeignKey("EndAddress")]
        public int EndAddressId { get; set; }
        required public virtual Address EndAddress { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        public virtual User? Driver { get; set; }
        public DateTime? EstimatedArrivalTime { get; set; } //Estimated time of arrival of Driver to start address
        public DateTime? EstimatedRideTime { get; set; }    //Estimated time of Ride
        public long RidePrice { get; set; }

        public RideStatus Status { get; set; }
    }
}