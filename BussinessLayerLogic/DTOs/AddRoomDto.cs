using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayerLogic.DTOs
{
    public class AddRoomDto
    {
        public string RoomNumber { get; set; } = string.Empty;
        public int FloorNumber { get; set; }
        public string RoomTypeName { get; set; }
    }
}
