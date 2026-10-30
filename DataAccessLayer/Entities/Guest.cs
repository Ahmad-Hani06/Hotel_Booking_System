using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Guest
{
    public int GuestId { get; set; }

    public int PersonId { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string? Notes { get; set; }

    public virtual Person Person { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
