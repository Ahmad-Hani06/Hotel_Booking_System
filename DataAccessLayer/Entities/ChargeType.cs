using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class ChargeType
{
    public int ChargeTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual ICollection<ReservationCharge> ReservationCharges { get; set; } = new List<ReservationCharge>();
}
