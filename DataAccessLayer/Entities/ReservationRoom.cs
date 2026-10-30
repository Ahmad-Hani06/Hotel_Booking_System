using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class ReservationRoom
{
    public int ReservationId { get; set; }

    public int RoomId { get; set; }

    public decimal PricePerNightAtBooking { get; set; }

    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
