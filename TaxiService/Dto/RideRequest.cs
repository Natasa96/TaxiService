using DataLib.Model;
using Microsoft.IdentityModel.Tokens;

public class RideRequest
{
  public DateTime StartTime { get; set; }
  public DateTime EndTime { get; set; }
  public AddressDto StartAddress { get; set; }
  public AddressDto EndAddress { get; set; }
  public int UserId { get; set; }
  public int DriverId { get; set; }
}

public class AddressDto
{
  public string StreetName { get; set; }
  public string StreetNumber { get; set; }
  public string City { get; set; }
  public int ZipCode { get; set; }
}