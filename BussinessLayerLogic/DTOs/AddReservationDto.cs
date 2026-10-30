using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BussinessLayerLogic.DTOs
{
    public class AddReservationDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^\+?[0-9\s\-()]{7,20}$", ErrorMessage = "Phone number format is invalid.")]
        public string PhoneNumber { get; set; }


        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        public List<int> roomIds { get; set; } = new();

        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int UserId { get; set; }

    }
}
