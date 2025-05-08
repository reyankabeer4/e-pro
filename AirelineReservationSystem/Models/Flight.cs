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

    public string? FlightPic { get; set; }

    public string? Operatingairline { get; set; }

    public string? Flightduration { get; set; }

    public int? Numberofstops { get; set; }


    // Flight Duration calculated based on departure and arrival times
    public TimeSpan FlightDuration
    {
        get
        {
            if (DepartureTime.HasValue && ArrivalTime.HasValue)
            {
                return ArrivalTime.Value - DepartureTime.Value;
            }
            return TimeSpan.Zero;
        }
    }

    // Formatted Flight Duration as a string (e.g., "2h 30m")
    public string GetFormattedDuration()
    {
        var duration = FlightDuration;
        return $"{(int)duration.TotalHours}h {duration.Minutes}m";
    }


    public void SetNumberOfStops(int stops)
    {
        Numberofstops = stops;
    }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
