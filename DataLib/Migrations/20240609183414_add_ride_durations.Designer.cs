﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataLib.Migrations
{
    [DbContext(typeof(TaxiDbContext))]
    [Migration("20240609183414_add_ride_durations")]
    partial class add_ride_durations
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DataLib.Model.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("longtext");

                    b.Property<string>("StreetName")
                        .HasColumnType("longtext");

                    b.Property<string>("StreetNumber")
                        .HasColumnType("longtext");

                    b.Property<int?>("ZipCode")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("DataLib.Model.Ride", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime?>("AcceptedTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("DriverId")
                        .HasColumnType("int");

                    b.Property<int>("EndAddressId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("EstimatedArrivalTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("EstimatedRideTime")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("RidePrice")
                        .HasColumnType("bigint");

                    b.Property<int>("StartAddressId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.HasIndex("EndAddressId");

                    b.HasIndex("StartAddressId");

                    b.HasIndex("UserId");

                    b.ToTable("Rides");
                });

            modelBuilder.Entity("DataLib.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Picture")
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Surname")
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("VerificationState")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Rating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("DriverId")
                        .HasColumnType("int");

                    b.Property<double>("Rate")
                        .HasColumnType("double");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.HasIndex("UserId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("DataLib.Model.Ride", b =>
                {
                    b.HasOne("DataLib.Model.User", "Driver")
                        .WithMany()
                        .HasForeignKey("DriverId");

                    b.HasOne("DataLib.Model.Address", "EndAddress")
                        .WithMany()
                        .HasForeignKey("EndAddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataLib.Model.Address", "StartAddress")
                        .WithMany()
                        .HasForeignKey("StartAddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataLib.Model.User", "User")
                        .WithMany("Rides")
                        .HasForeignKey("UserId");

                    b.Navigation("Driver");

                    b.Navigation("EndAddress");

                    b.Navigation("StartAddress");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataLib.Model.User", b =>
                {
                    b.HasOne("DataLib.Model.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.Navigation("Address");
                });

            modelBuilder.Entity("Rating", b =>
                {
                    b.HasOne("DataLib.Model.User", "Driver")
                        .WithMany("DriverRatings")
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DataLib.Model.User", "User")
                        .WithMany("UserRatings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Driver");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataLib.Model.User", b =>
                {
                    b.Navigation("DriverRatings");

                    b.Navigation("Rides");

                    b.Navigation("UserRatings");
                });
#pragma warning restore 612, 618
        }
    }
}
