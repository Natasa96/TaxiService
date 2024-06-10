using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLib.Model;

public class User
{
    [Key]
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public DateTime Birthday { get; set; }
    [ForeignKey("Address")]
    public int? AddressId { get; set; }
    public virtual Address? Address { get; set; }
    public string? Picture { get; set; }
    public Roles Role { get; set; }
    public VerificationState VerificationState { get; set; }
    public virtual ICollection<Ride>? Rides { get; set; }
    public virtual ICollection<Rating>? UserRatings { get; set; }
    public virtual ICollection<Rating>? DriverRatings { get; set; }

    public override string ToString()
    {
        return this.Name + " " + this.Surname;
    }
}