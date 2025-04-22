using System;
using System.Collections.Generic;

namespace AirlineReservationSystem.Models;

public partial class Flight
{
    public int Flightid { get; set; }

    public string Flightnumber { get; set; } = null!;

    public string? Airline { get; set; }

    public int? Fromairportid { get; set; }

    public int? Toairportid { get; set; }

    public DateTime Departuretime { get; set; }

    public DateTime Arrivaltime { get; set; }

    public int Seatcapacity { get; set; }

    public virtual Airport? Fromairport { get; set; }

    public virtual ICollection<Reschedule> Reschedules { get; set; } = new List<Reschedule>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual Airport? Toairport { get; set; }
}
