namespace BussinessLayerLogic.DTOs;

public class GuestReservationResponseDto
{
    public int ReservationId { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string ReservationStatus { get; set; } = string.Empty;
    public List<string> RoomNumbers { get; set; } = new();
}
