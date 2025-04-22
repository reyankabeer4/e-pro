using System;
using System.Collections.Generic;

namespace AirlineReservationSystem.Models;

public partial class Reschedule
{
    public int Rescheduleid { get; set; }

    public int? Ticketid { get; set; }

    public int? Oldflightid { get; set; }

    public int? Newflightid { get; set; }

    public DateTime? Requestedat { get; set; }

    public bool? Approved { get; set; }

    public virtual Flight? Newflight { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
