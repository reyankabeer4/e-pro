using System;
using System.Collections.Generic;

namespace AirlineReservationSystem.Models;

public partial class Airport
{
    public int Airportid { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? City { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<Flight> FlightFromairports { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightToairports { get; set; } = new List<Flight>();
}
