using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class RoomType
{
    public int RoomTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public decimal PricePerNight { get; set; }

    public int MaxOccupancy { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
