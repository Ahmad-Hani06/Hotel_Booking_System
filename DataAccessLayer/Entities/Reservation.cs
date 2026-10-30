using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int GuestId { get; set; }

    public DateTime ReservationDate { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string ReservationStatus { get; set; } = null!;

    public virtual Guest Guest { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<ReservationCharge> ReservationCharges { get; set; } = new List<ReservationCharge>();

    public virtual ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();
}
