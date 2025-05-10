using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AirelineReservationSystem.Models;

public partial class NeondbContext : DbContext
{
    public NeondbContext()
    {
    }

    public NeondbContext(DbContextOptions<NeondbContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<Passenger> Passengers { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=ep-proud-grass-a4junec8-pooler.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_lGyNPXmoA92H;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("bookings_pkey");

            entity.ToTable("bookings");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("booking_date");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Flight).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.FlightId)
                .HasConstraintName("bookings_flight_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("bookings_user_id_fkey");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.FlightId).HasName("flights_pkey");

            entity.ToTable("flights");

            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.AirlineName)
                .HasMaxLength(100)
                .HasColumnName("airline_name");
            entity.Property(e => e.ArrivalTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("arrival_time");
            entity.Property(e => e.AvailableSeats).HasColumnName("available_seats");
            entity.Property(e => e.DepartureTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("departure_time");
            entity.Property(e => e.Destination)
                .HasMaxLength(100)
                .HasColumnName("destination");
            entity.Property(e => e.FlightNumber)
                .HasMaxLength(20)
                .HasColumnName("flight_number");
            entity.Property(e => e.FlightPic)
                .HasMaxLength(50)
                .HasColumnName("flight_pic");
            entity.Property(e => e.Flightduration)
                .HasMaxLength(20)
                .HasColumnName("flightduration");
            entity.Property(e => e.Numberofstops).HasColumnName("numberofstops");
            entity.Property(e => e.Operatingairline)
                .HasMaxLength(50)
                .HasColumnName("operatingairline");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .HasColumnName("source");
            entity.Property(e => e.TotalSeats).HasColumnName("total_seats");
        });

        modelBuilder.Entity<Passenger>(entity =>
        {
            entity.HasKey(e => e.PassengerId).HasName("passengers_pkey");

            entity.ToTable("passengers");

            entity.Property(e => e.PassengerId).HasColumnName("passenger_id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(20)
                .HasColumnName("passport_number");

            entity.HasOne(d => d.Booking).WithMany(p => p.Passengers)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("passengers_booking_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasColumnName("payment_status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("payments_booking_id_fkey");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("seats_pkey");

            entity.ToTable("seats");

            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.IsBooked)
                .HasDefaultValue(false)
                .HasColumnName("is_booked");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(10)
                .HasColumnName("seat_number");

            entity.HasOne(d => d.Flight).WithMany(p => p.Seats)
                .HasForeignKey(d => d.FlightId)
                .HasConstraintName("seats_flight_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.ProfilePic)
                .HasMaxLength(255)
                .HasColumnName("profile_pic");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.Sex)
                .HasMaxLength(6)
                .HasColumnName("sex");
            entity.Property(e => e.Skymiles)
                .HasDefaultValue(0)
                .HasColumnName("skymiles");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
