using System;
using System.Collections.Generic;

namespace AirlineReservationSystem.Models;

public partial class Ticket
{
    public int Ticketid { get; set; }

    public int? Flightid { get; set; }

    public int? Userid { get; set; }

    public DateTime? Bookingdate { get; set; }

    public string? Status { get; set; }

    public string? Seatnumber { get; set; }

    public string? Class { get; set; }

    public virtual Flight? Flight { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Reschedule> Reschedules { get; set; } = new List<Reschedule>();

    public virtual User? User { get; set; }
}
