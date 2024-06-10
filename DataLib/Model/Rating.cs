using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLib.Model;

public class Rating
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    [ForeignKey("Driver")]
    public int DriverId { get; set; }
    public virtual User? Driver { get; set; }
    public double Rate { get; set; }
}