using BussinessLayerLogic.DTOs;
using BussinessLayerLogic.Exceptions;
using BussinessLayerLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Booking_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GuestController : ControllerBase
{
    private readonly GuestService _guestService;

    public GuestController(GuestService guestService)
    {
        _guestService = guestService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGuests()
    {
        return Ok(await _guestService.GetAllGuestsAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGuestById(int id)
    {
        try
        {
            var guest = await _guestService.GetGuestByIdAsync(id);
            return guest == null ? NotFound("Guest not found.") : Ok(guest);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchGuests([FromQuery] string query)
    {
        try
        {
            return Ok(await _guestService.SearchGuestsAsync(query));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddGuest([FromBody] AddGuestDto guestDto)
    {
        try
        {
            var guest = await _guestService.AddGuestAsync(guestDto);
            return CreatedAtAction(nameof(GetGuestById), new{ id = guest.GuestId }, guest);
        }
        catch (DuplicateGuestException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGuest(int id, [FromBody] UpdateGuestDto guestDto)
    {
        try
        {
            bool updated = await _guestService.UpdateGuestAsync(id, guestDto);
            return updated ? NoContent() : NotFound("Guest not found.");
        }
        catch (DuplicateGuestException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGuest(int id)
    {
        try
        {
            bool deleted = await _guestService.DeleteGuestAsync(id);
            return deleted ? NoContent() : NotFound("Guest not found.");
        }
        catch (GuestHasReservationsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //[HttpGet("{id:int}/reservations")]
    //public async Task<IActionResult> GetGuestReservations(int id)
    //{
    //    try
    //    {
    //        var reservations = await _guestService.GetGuestReservationsAsync(id);
    //        return reservations == null ? NotFound("Guest not found.") : Ok(reservations);
    //    }
    //    catch (ArgumentOutOfRangeException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}
}
