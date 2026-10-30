using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int ReservationId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
