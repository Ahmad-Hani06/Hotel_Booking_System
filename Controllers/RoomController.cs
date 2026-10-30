using BussinessLayerLogic.DTOs;
using BussinessLayerLogic.Exceptions;
using BussinessLayerLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Hotel_Booking_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("GetAllRooms")]

        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(rooms);
        }


        [HttpGet("GetRoomByRoomNumber")]

        public async Task<IActionResult> GetRoomByRoomNumber( string roomNumber)
        {
            try
            {
                var room = await _roomService.GetRoomByRoomNumberAsync(roomNumber);

                if (room == null)
                    return NotFound($"This room number {roomNumber} does not exists");

                return Ok(room);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetAvailableRooms")]
        public async Task<IActionResult> GetAvailableRooms(DateOnly checkInDate, DateOnly checkOutDate)
        {
            var availableRooms = await _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate);

            if (availableRooms.Count == 0)
                return NotFound("No available rooms for the selected dates.");

            return Ok(availableRooms);
        }


        [HttpPost("AddRoom")]

        public async Task<IActionResult> AddRoom(AddRoomDto roomDto)
        {
            try
            {
                int? roomId = await _roomService.AddRoomAsync(roomDto);

                if (roomId == null)
                    return BadRequest("RoomTypeId does not exists in the system");

                return StatusCode(201, new { RoomId = roomId });
            }

            catch (DuplicateRoomNumberException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
               return BadRequest(ex.Message);
            }

        }
        
        [HttpPut("UpdateRoomType")]

        public async Task<IActionResult> UpdateRoomTypeIdByRoomId(int roomId, string RoomType)
        {
            bool result = await _roomService.UpdateRoomTypeIdByRoomIdAsync(roomId, RoomType);
            if (!result)
                return NotFound("This RoomType does not exist");

            return NoContent();

        }


    }
}
