using System;
using System.Collections.Generic;

namespace AirelineReservationSystem.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? FlightId { get; set; }

    public DateTime? BookingDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; }

    public virtual Flight? Flight { get; set; }

    public virtual ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
