namespace BussinessLayerLogic.DTOs;

public class GuestResponseDto
{
    public int GuestId { get; set; }
    public int PersonId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string? Notes { get; set; }
}
