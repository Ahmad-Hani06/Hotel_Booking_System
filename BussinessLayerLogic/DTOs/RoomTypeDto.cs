using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BussinessLayerLogic.DTOs
{
    public class RoomTypeDto
    {
        public string typeName { get; set; } = string.Empty;
        public decimal pricePerNight { get; set; }
        public int maxOccupancy { get; set; }
        public string Description { get; set; } = string.Empty;


    }
}
