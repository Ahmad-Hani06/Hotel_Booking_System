using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Room
{
    public int RoomId { get; set; }

    public int RoomTypeId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public int FloorNumber { get; set; }

    public virtual ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();

    public virtual RoomType RoomType { get; set; } = null!;
}
