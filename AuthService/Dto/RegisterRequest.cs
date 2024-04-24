using DataLib.Model;

public class RegisterRequest
{
  public string User_name { get; set; }
  public string Password { get; set; }
  public string Name { get; set; }
  public string Surname { get; set; }
  public string Email { get; set; }
  public DateTime Birthday { get; set; }
  public AddressDto Address { get; set; }
  public string Picture { get; set; }
  public RolesDto Role { get; set; }
}

public class AddressDto
{
  public string StreetName { get; set; }
  public string StreetNumber { get; set; }
  public string City { get; set; }
  public int ZipCode { get; set; }
}

public enum RolesDto
{
  User = Roles.User,
  Driver = Roles.Driver
}
