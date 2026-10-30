using System;
using System.Collections.Generic;
using System.Text;
using BussinessLayerLogic.DTOs;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Entities;

namespace BussinessLayerLogic.Services
{
    public class RoomTypeService
    {
        private readonly RoomTypeData _roomTypeData;

        public RoomTypeService(RoomTypeData roomTypeData)
        {
            _roomTypeData = roomTypeData;
        }

        public async Task<int?> AddRoomTypeAsync(RoomTypeDto roomTypeDto)
        {
            if (roomTypeDto == null)
                throw new ArgumentNullException(nameof(roomTypeDto), "RoomTypeDto cannot be null.");

            
            if (string.IsNullOrWhiteSpace(roomTypeDto.typeName))
                throw new ArgumentException("Room type name cannot be empty or whitespace.", nameof(roomTypeDto.typeName));


            string name = roomTypeDto.typeName.Trim();


            if (roomTypeDto.pricePerNight <= 0)
                throw new ArgumentOutOfRangeException(nameof(roomTypeDto.pricePerNight), "Price per night must be greater than zero");


            if (roomTypeDto.maxOccupancy <= 0)
                throw new ArgumentOutOfRangeException(nameof(roomTypeDto.maxOccupancy),"Max occupancy must be greater than zero.");

            if (await _roomTypeData.ExistsByNameAsync(name))
                return null;

            RoomType roomType = new RoomType
            {
                TypeName = name,
                PricePerNight = roomTypeDto.pricePerNight,
                MaxOccupancy = roomTypeDto.maxOccupancy,
                Description = roomTypeDto.Description
            };

           int roomTypeId = await _roomTypeData.AddRoomTypeAsync(roomType);

            return roomTypeId;
        }

        public async Task<List<RoomTypeResponseDto>> GetAllRoomTypesAsync()
        {
            List<RoomTypeResponseDto> roomTypes = new List<RoomTypeResponseDto>();

            var roomTypeEntities = await _roomTypeData.GetAllRoomTypesAsync();

            foreach (var roomType in roomTypeEntities)
            {
                    RoomTypeResponseDto dto = new RoomTypeResponseDto
                    {
                        RoomTypeId = roomType.RoomTypeId,
                        TypeName = roomType.TypeName,
                        PricePerNight = roomType.PricePerNight,
                        MaxOccupancy = roomType.MaxOccupancy,
                        Description = roomType.Description
                    };

                roomTypes.Add(dto);
            }

            return roomTypes;
        }

        public async Task<RoomTypeResponseDto?> GetRoomTypeByIdAsync(int roomTypeId)
        {
            if (roomTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(roomTypeId), "Room type ID must be greater than zero.");
            }

            var roomType = await _roomTypeData.GetRoomTypeByIdAsync(roomTypeId);

            if (roomType == null)
            {
                return null;
            }


            RoomTypeResponseDto dto = new RoomTypeResponseDto
            {
                RoomTypeId = roomType.RoomTypeId,
                TypeName = roomType.TypeName,
                PricePerNight = roomType.PricePerNight,
                MaxOccupancy = roomType.MaxOccupancy,
                Description = roomType.Description
            };

            return dto;
        }


        public async Task<bool> UpdateRoomTypeByIdAsync(int roomTypeId , RoomTypeDto roomTypeDto)
        {
            if (roomTypeDto == null)
                throw new ArgumentNullException(nameof(roomTypeDto), "roomTypeDto does not allow null");

            if (roomTypeId <= 0)
                throw new ArgumentOutOfRangeException(nameof(roomTypeId), $"roomTypeId = {roomTypeId} should be grater than zero");


            if (string.IsNullOrWhiteSpace(roomTypeDto.typeName))
                throw new ArgumentException("Invalid room type name.");

            if (roomTypeDto.pricePerNight <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(roomTypeDto.pricePerNight));

            if (roomTypeDto.maxOccupancy <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(roomTypeDto.maxOccupancy));


            if (!await _roomTypeData.ExistsByIdAsync(roomTypeId))
            {
                return false; // we put fasle because this case is not an error, it just means that the room type with the given ID does not exist
            }

            RoomType roomType = new RoomType
            {
                RoomTypeId = roomTypeId,
                TypeName = roomTypeDto.typeName.Trim(),
                PricePerNight = roomTypeDto.pricePerNight,
                MaxOccupancy = roomTypeDto.maxOccupancy,
                Description = roomTypeDto.Description
            };

            return await _roomTypeData.UpdateRoomTypeByIdAsync(roomTypeId, roomType);
        }
    }
}
