using DataAccessLayer.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayerLogic.DTOs
{
    public class ReservationDetailsDto
    {
        public int ReservationId { get; set; }

        // Guest Information
        public string GuestFullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }

        // Reservation Information
        public DateTime ReservationDate { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public string ReservationStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        // Booked Rooms
        public List<RoomResponseDto> Rooms { get; set; } = new();
    }
}
