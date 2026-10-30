using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayerLogic.DTOs
{
    public class UpdateReservationDto
    {
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public string ReservationStatus { get; set; } = "Confirmed";
        public List<int> RoomIds { get; set; } = new();
        public int CreatedByUserId { get; set; }

    }
}
