using System;

namespace DataLib.Model;

public class User
{
    public int Id { get; set; }
    public string User_name { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime Birthday { get; set; }
    public Address Address { get; set; }
    public string Picture { get; set; }
    public Roles Role { get; set; }
    public VerificationState VerificationState { get; set; }
}