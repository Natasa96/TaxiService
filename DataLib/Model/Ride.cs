using System;
using DataLib;

namespace DataLib.Model
{
    public class Ride
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Address StartAddress { get; set; }
        public Address EndAddress { get; set; }
        public int UserId { get; set; }
        public int DriverId { get; set; }
    }
}