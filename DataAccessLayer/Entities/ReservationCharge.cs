using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class ReservationCharge
{
    public int ReservationChargeId { get; set; }

    public int ReservationId { get; set; }

    public int ChargeTypeId { get; set; }

    public int UserId { get; set; }

    public DateTime ChargedAt { get; set; }

    public decimal Amount { get; set; }

    public virtual ChargeType ChargeType { get; set; } = null!;

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
