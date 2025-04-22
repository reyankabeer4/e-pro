using System;
using System.Collections.Generic;

namespace AirlineReservationSystem.Models;

public partial class User
{
    public int Userid { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string? Phonenumber { get; set; }

    public string? Role { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
