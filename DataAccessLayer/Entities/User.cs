using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class User
{
    public int UserId { get; set; }

    public int PersonId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Person Person { get; set; } = null!;

    public virtual ICollection<ReservationCharge> ReservationCharges { get; set; } = new List<ReservationCharge>();

    public virtual ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();
}
