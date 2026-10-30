
namespace BussinessLayerLogic.DTOs
{
    public class RoomResponseDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public int FloorNumber { get; set; }

        // Room Type Information
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxOccupancy { get; set; }
    }
}
