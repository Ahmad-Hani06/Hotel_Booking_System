using BussinessLayerLogic.DTOs;
using BussinessLayerLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Booking_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationService _reservationService;
        public ReservationController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost("AddReservation")]

        public async Task<IActionResult> AddReservation(AddReservationDto reservationDto)
        {
            try
            {
                int reservationId = await _reservationService.AddReservation(reservationDto);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    Message = "Reservation created successfully.",
                    ReservationId = reservationId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPatch("{reservationId:int}/Cancel")]

        public async Task<IActionResult> CancelledReservation(int reservationId)
        {
            try
            {
                bool isCancelled = await _reservationService
                    .CancelledReservationAsync(reservationId);

                if (!isCancelled)
                    return Conflict("Reservation could not be cancelled.");

                return NoContent();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }

        }
        [HttpGet("{reservationId:int}")]
        public async Task<IActionResult> GetReservationById(int reservationId)
        {
            try
            {
                var reservation = await _reservationService
                    .GetReservationDetailsByReservationIdAsync(reservationId);

                if (reservation == null)
                    return NotFound($"Reservation with ID {reservationId} was not found.");

                return Ok(reservation);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateReservation/{reservationId}")]
        public async Task<IActionResult> UpdateReservation(
    int reservationId,
    [FromBody] UpdateReservationDto reservationDto)
        {
            try
            {
                bool isUpdated = await _reservationService
                    .UpdateReservationByReservationIdAsync(reservationId, reservationDto);

                if (!isUpdated)
                    return NotFound("Reservation not found.");

                return Ok("Reservation updated successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("{reservationId:int}/check-in")]
        public async Task<IActionResult> CheckInReservation(int reservationId)
        {
            if (reservationId <= 0)
                return BadRequest("Invalid reservation ID.");

            try
            {
                bool isCheckedIn = await _reservationService
                    .CheckInReservationAsync(reservationId);

                if (!isCheckedIn)
                    return NotFound("Reservation not found.");

                return Ok("Reservation checked in successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("{reservationId:int}/check-out")]

        public async Task<IActionResult> CheckOutReservation(int reservationId)
        {
            if (reservationId <= 0)
                return BadRequest("Invalid reservation ID.");

            try
            {
                bool isCheckedOut = await _reservationService.CheckOutReservationAsync(reservationId);

                if (!isCheckedOut)
                    return NotFound("Reservation not found.");

                return Ok("Reservation checked out successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }


    }
}
