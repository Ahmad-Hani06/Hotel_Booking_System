using System.ComponentModel.DataAnnotations;

namespace BussinessLayerLogic.DTOs;

public class AddGuestDto
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [RegularExpression(@"^\+?[0-9\s\-()]{7,20}$", ErrorMessage = "Phone number format is invalid.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
