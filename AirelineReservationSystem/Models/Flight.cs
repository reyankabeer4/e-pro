using System;
using System.Collections.Generic;

namespace AirelineReservationSystem.Models;

public partial class Flight
{
    public int FlightId { get; set; }

    public string? AirlineName { get; set; }

    public string? FlightNumber { get; set; }

    public string? Source { get; set; }

    public string? Destination { get; set; }

    public DateTime? DepartureTime { get; set; }

    public DateTime? ArrivalTime { get; set; }

    public int? TotalSeats { get; set; }

    public int? AvailableSeats { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
