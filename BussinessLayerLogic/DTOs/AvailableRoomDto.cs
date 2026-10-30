using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayerLogic.DTOs
{
    public class AvailableRoomDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int FloorNumber { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
    }
}
