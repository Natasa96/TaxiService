using System;
using System.ComponentModel.DataAnnotations;


namespace DataLib.Model
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string? StreetName { get; set; }
        public string? StreetNumber { get; set; }
        public string? City { get; set; }
        public int? ZipCode { get; set; }
        public override string ToString()
        {
            return StreetName + ", " + StreetNumber + ", " + City + ", " + ZipCode;
        }
    }
}