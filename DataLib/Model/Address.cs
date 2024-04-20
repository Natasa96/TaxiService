using System;


namespace DataLib.Model
{
    public class Address
    {
        public int Id { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
    }
}