using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AirlineReservationSystem.Models;

public partial class NeondbContext : DbContext
{
    public NeondbContext()
    {
    }

    public NeondbContext(DbContextOptions<NeondbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Reschedule> Reschedules { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseNpgsql("Host=ep-withered-block-a40v07w4-pooler.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_apSO7kxn6ofC;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Airportid).HasName("airports_pkey");

            entity.ToTable("airports");

            entity.HasIndex(e => e.Code, "airports_code_key").IsUnique();

            entity.Property(e => e.Airportid).HasColumnName("airportid");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Flightid).HasName("flights_pkey");

            entity.ToTable("flights");

            entity.HasIndex(e => e.Flightnumber, "flights_flightnumber_key").IsUnique();

            entity.Property(e => e.Flightid).HasColumnName("flightid");
            entity.Property(e => e.Airline)
                .HasMaxLength(50)
                .HasColumnName("airline");
            entity.Property(e => e.Arrivaltime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("arrivaltime");
            entity.Property(e => e.Departuretime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("departuretime");
            entity.Property(e => e.Flightnumber)
                .HasMaxLength(20)
                .HasColumnName("flightnumber");
            entity.Property(e => e.Fromairportid).HasColumnName("fromairportid");
            entity.Property(e => e.Seatcapacity).HasColumnName("seatcapacity");
            entity.Property(e => e.Toairportid).HasColumnName("toairportid");

            entity.HasOne(d => d.Fromairport).WithMany(p => p.FlightFromairports)
                .HasForeignKey(d => d.Fromairportid)
                .HasConstraintName("flights_fromairportid_fkey");

            entity.HasOne(d => d.Toairport).WithMany(p => p.FlightToairports)
                .HasForeignKey(d => d.Toairportid)
                .HasConstraintName("flights_toairportid_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.Property(e => e.Paymentid).HasColumnName("paymentid");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Paymentdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paymentdate");
            entity.Property(e => e.Paymentmethod)
                .HasMaxLength(30)
                .HasColumnName("paymentmethod");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Ticketid).HasColumnName("ticketid");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Ticketid)
                .HasConstraintName("payments_ticketid_fkey");
        });

        modelBuilder.Entity<Reschedule>(entity =>
        {
            entity.HasKey(e => e.Rescheduleid).HasName("reschedules_pkey");

            entity.ToTable("reschedules");

            entity.Property(e => e.Rescheduleid).HasColumnName("rescheduleid");
            entity.Property(e => e.Approved)
                .HasDefaultValue(false)
                .HasColumnName("approved");
            entity.Property(e => e.Newflightid).HasColumnName("newflightid");
            entity.Property(e => e.Oldflightid).HasColumnName("oldflightid");
            entity.Property(e => e.Requestedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("requestedat");
            entity.Property(e => e.Ticketid).HasColumnName("ticketid");

            entity.HasOne(d => d.Newflight).WithMany(p => p.Reschedules)
                .HasForeignKey(d => d.Newflightid)
                .HasConstraintName("reschedules_newflightid_fkey");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Reschedules)
                .HasForeignKey(d => d.Ticketid)
                .HasConstraintName("reschedules_ticketid_fkey");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Ticketid).HasName("tickets_pkey");

            entity.ToTable("tickets");

            entity.Property(e => e.Ticketid).HasColumnName("ticketid");
            entity.Property(e => e.Bookingdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("bookingdate");
            entity.Property(e => e.Class)
                .HasMaxLength(20)
                .HasColumnName("class");
            entity.Property(e => e.Flightid).HasColumnName("flightid");
            entity.Property(e => e.Seatnumber)
                .HasMaxLength(10)
                .HasColumnName("seatnumber");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Booked'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Flight).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.Flightid)
                .HasConstraintName("tickets_flightid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("tickets_userid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(20)
                .HasColumnName("phonenumber");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Customer'::character varying")
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
