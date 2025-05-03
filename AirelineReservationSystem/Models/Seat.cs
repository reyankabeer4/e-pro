using System;
using System.Collections.Generic;

namespace AirelineReservationSystem.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int? FlightId { get; set; }

    public string? SeatNumber { get; set; }

    public bool? IsBooked { get; set; }

    public virtual Flight? Flight { get; set; }
}
