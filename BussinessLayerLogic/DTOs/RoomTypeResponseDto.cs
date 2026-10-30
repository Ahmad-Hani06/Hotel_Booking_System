
namespace BussinessLayerLogic.DTOs
{
    public class RoomTypeResponseDto
    {
        public int RoomTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxOccupancy { get; set; }
        public string? Description { get; set; }
    }
}
