﻿using Microsoft.EntityFrameworkCore;
using DataLib.Model;

public class TaxiDbContext : DbContext
{
    // protected override void OnModelCreating(DbContextOptionsBuilder optionsBuilder){
    //     optionsBuilder.UseMySQL(Configuration.)
    // }
    public TaxiDbContext(DbContextOptions<TaxiDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Ride> Rides { get; set; }
    public DbSet<Address> Addresses { get; set; }

    /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     {
         optionsBuilder.UseSqlServer("mysql");
     }*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(p => p.Role)
            .HasConversion<string>();
        modelBuilder.Entity<User>()
            .Property(p => p.VerificationState)
            .HasConversion<string>();
    }
}