using BussinessLayerLogic.DTOs;
using BussinessLayerLogic.Exceptions;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace BussinessLayerLogic.Services
{
    public class RoomService
    {
        private readonly RoomData _roomData;
        private readonly RoomTypeData _roomTypeData;
        private readonly ReservationData _reservationData;
        public RoomService(RoomData roomData, RoomTypeData roomTypeData, ReservationData reservationData)
        {
            _roomData = roomData;
            _roomTypeData = roomTypeData;
            _reservationData = reservationData;
        }
        public async Task<List<RoomResponseDto>> GetAllRoomsAsync()
        {
            List<RoomResponseDto> dto = new List<RoomResponseDto>();

            var Rooms = await _roomData.GetAllRoomsAsync();

            // Now we need to check if Rooms is null or empty before proceeding to map the data to RoomResponseDto

            if (!Rooms.Any())
                return dto; // Return an empty list if there are no rooms


            foreach (var room in Rooms)
            {  
              RoomResponseDto roomResponseDto = new RoomResponseDto
              {
                  RoomId = room.RoomId,
                  RoomNumber = room.RoomNumber,
                  FloorNumber = room.FloorNumber,
                  RoomTypeName = room.RoomType.TypeName,
                  PricePerNight = room.RoomType.PricePerNight,
                  MaxOccupancy = room.RoomType.MaxOccupancy
              };

                dto.Add(roomResponseDto);
            }

            return dto;

        }
        public async Task<RoomResponseDto?> GetRoomByRoomNumberAsync(string roomNumber)
        {
            if (string.IsNullOrWhiteSpace(roomNumber))
                throw new ArgumentNullException(nameof(roomNumber), "room number should be a vaild value");

            var room = await _roomData.GetRoomByRoomNumberAsync(roomNumber);

            if (room == null)
                return null;

            RoomResponseDto roomResponse = new RoomResponseDto
            {
                RoomId = room.RoomId,
                FloorNumber = room.FloorNumber,
                MaxOccupancy = room.RoomType.MaxOccupancy,
                PricePerNight = room.RoomType.PricePerNight,
                RoomNumber = room.RoomNumber,
                RoomTypeName = room.RoomType.TypeName,
            };

            return roomResponse;
        }
        public async Task<bool> UpdateRoomTypeIdByRoomIdAsync(int roomId, string name)
        {

           int? roomTypeId = await _roomTypeData.GetRoomTypeIdByName(name);

            if (roomTypeId == null)
                return false;

            return await _roomData.UpdateRoomTypeIdByRoomIdAsync(roomId, roomTypeId.Value);
        }
        private async Task<bool> CheckIfRoomExists(string roomNumber)
        {
           return await _roomData.ExistsByRoomNumberAsync(roomNumber);
        }
        public async Task<int?> AddRoomAsync(AddRoomDto roomDto)
        {
            ArgumentNullException.ThrowIfNull(roomDto);

            if (string.IsNullOrWhiteSpace(roomDto.RoomNumber))
                throw new ArgumentException("Room number is required.");

            if (string.IsNullOrWhiteSpace(roomDto.RoomTypeName))
                throw new ArgumentException("Room type name is required.");

            string roomNumber = roomDto.RoomNumber.Trim();
            string roomTypeName = roomDto.RoomTypeName.Trim();

            if (await CheckIfRoomExists(roomNumber))
                throw new DuplicateRoomNumberException(roomNumber);

            int? roomTypeId = await _roomTypeData.GetRoomTypeIdByName(roomTypeName);

            if (roomTypeId == null)
                return null;

            Room room = new Room
            {
                RoomNumber = roomNumber,
                FloorNumber = roomDto.FloorNumber,
                RoomTypeId = roomTypeId.Value
            };

            return await _roomData.AddRoomAsync(room);
        }

        public async Task<List<AvailableRoomDto>> GetAvailableRoomsAsync(
       DateOnly checkInDate, DateOnly checkOutDate)
        {
            if (checkOutDate <= checkInDate)
                throw new ArgumentException(
                    "Check-out date must be after check-in date.");

            if (checkInDate < DateOnly.FromDateTime(DateTime.Today))
                throw new ArgumentException(
                    "Check-in date cannot be in the past.");

            var bookedRoomIds = await _reservationData
                .BookedRoomsAsync(checkInDate, checkOutDate);

            var availableRooms = await _roomData
                .GetAvailableRoomsAsync(bookedRoomIds);

            return availableRooms.Select(r => new AvailableRoomDto
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                FloorNumber = r.FloorNumber,
                RoomTypeName = r.RoomType.TypeName,
                PricePerNight = r.RoomType.PricePerNight
            }).ToList();
        }
    }
}
