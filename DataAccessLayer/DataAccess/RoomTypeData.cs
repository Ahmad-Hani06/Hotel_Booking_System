using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.DataAccess
{
    public class RoomTypeData
    {
        private readonly HotelBookingDbContext _context;

        public RoomTypeData(HotelBookingDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddRoomTypeAsync(RoomType roomType)
        {
            _context.RoomTypes.Add(roomType);

            await _context.SaveChangesAsync();

            return roomType.RoomTypeId;
        }

        public async Task<bool> ExistsByNameAsync(string typeName)
        {
            return await _context.RoomTypes.AnyAsync(tn => tn.TypeName == typeName);
        }

        public async Task<bool> ExistsByIdAsync(int roomTypeId)
        {
            return await _context.RoomTypes.AnyAsync(tn => tn.RoomTypeId == roomTypeId);
        }

        public async Task<List<RoomType>> GetAllRoomTypesAsync()
        {
            return await _context.RoomTypes.AsNoTracking().ToListAsync();
        }

        public async Task<RoomType?> GetRoomTypeByIdAsync(int roomTypeId)
        {
            return await _context.RoomTypes.AsNoTracking().FirstOrDefaultAsync(rt => rt.RoomTypeId == roomTypeId);
        }

        public async Task<bool> UpdateRoomTypeByIdAsync(int roomTypeId, RoomType roomType)
        {
            var dbRoomType = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.RoomTypeId == roomTypeId); // One object is returned, or null if not found

            if (dbRoomType == null)
            {
                return false;
            }

            dbRoomType.TypeName = roomType.TypeName;
            dbRoomType.PricePerNight = roomType.PricePerNight;
            dbRoomType.MaxOccupancy = roomType.MaxOccupancy;
            dbRoomType.Description = roomType.Description;


            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<int?> GetRoomTypeIdByName(string name)
        {
            return await _context.RoomTypes.AsNoTracking().Where(rt => rt.TypeName == name).Select(rt => (int?)rt.RoomTypeId).FirstOrDefaultAsync();
        }
    }
}
