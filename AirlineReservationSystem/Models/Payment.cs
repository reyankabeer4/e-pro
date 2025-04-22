using System;
using System.Collections.Generic;

namespace AirlineReservationSystem.Models;

public partial class Payment
{
    public int Paymentid { get; set; }

    public int? Ticketid { get; set; }

    public decimal Amount { get; set; }

    public DateTime? Paymentdate { get; set; }

    public string? Paymentmethod { get; set; }

    public string? Status { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
