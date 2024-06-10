using Microsoft.EntityFrameworkCore;
using DataLib.Model;

public class TaxiDbContext : DbContext
{

    public TaxiDbContext(DbContextOptions<TaxiDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Ride> Rides { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     {
         optionsBuilder.UseSqlServer("mysql");
     }*/

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseMySQL("Server=mysql,3306;Database=taxiservicedb;User Id=admin;Password=admin;");
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(p => p.Role)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .Property(p => p.VerificationState)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .HasMany(r => r.Rides)
            .WithOne(r => r.User);

        modelBuilder.Entity<User>()
            .HasMany(u => u.UserRatings)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.DriverRatings)
            .WithOne(r => r.Driver)
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        // modelBuilder.Entity<Ride>()
        //     .HasOne(r => r.Driver)
        //     .WithMany(r => r.Rides)
        //     .HasForeignKey(r => r.DriverId)
        //     .IsRequired(false);

        // modelBuilder.Entity<Ride>()
        //     .HasOne(r => r.User)
        //     .WithMany(r => r.Rides)
        //     .HasForeignKey(r => r.UserId);
    }
}
