using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BussinessLayerLogic.DTOs;
using BussinessLayerLogic.Services;

namespace Hotel_Booking_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {

        private readonly RoomTypeService _roomTypeService;
        public RoomTypeController(RoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddRoomType([FromBody] RoomTypeDto roomTypeDto)
        {

            try
            {
                int? roomTypeId = await _roomTypeService.AddRoomTypeAsync(roomTypeDto);

                if (roomTypeId == null)
                {
                    return Conflict("Room type already exists.");
                }

                return StatusCode(StatusCodes.Status201Created, new { RoomTypeId = roomTypeId });

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoomTypes()
        {
            var roomTypes = await _roomTypeService.GetAllRoomTypesAsync();
            return Ok(roomTypes);
        }

        [HttpGet("id")]

        public async Task<IActionResult> GetRoomTypeById(int id)
        {
            try
            {
                var roomType = await _roomTypeService.GetRoomTypeByIdAsync(id);

                if (roomType == null)
                    return NotFound("Room type not found.");

                return Ok(roomType);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateRoomTypeById(int id, [FromBody] RoomTypeDto Dto)
        {
            try
            {
                bool isUpdated = await _roomTypeService.UpdateRoomTypeByIdAsync(id, Dto);

                return isUpdated ? NoContent() : NotFound("Room type not found.");
            }
            catch (ArgumentException ex)    
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
