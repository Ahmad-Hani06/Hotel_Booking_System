using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.DataAccess
{
    public class RoomData
    {
        private readonly HotelBookingDbContext _context;

        public RoomData(HotelBookingDbContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            var rooms = await _context.Rooms.AsNoTracking().Include(r => r.RoomType).ToListAsync();
            return rooms;
        }

        public async Task<Room?> GetRoomByRoomNumberAsync(string roomNumber)
        {
            return await _context.Rooms.AsNoTracking().Include(r => r.RoomType).FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
        }

        public async Task<bool> UpdateRoomTypeIdByRoomIdAsync(int roomId, int newRoomTypeId)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (room == null)
                return false;

            room.RoomTypeId = newRoomTypeId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> AddRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return room.RoomId;
        }


        public async Task<bool> ExistsByRoomNumberAsync(string roomNumber)
        {
            return await _context.Rooms.AsNoTracking().AnyAsync(r => r.RoomNumber == roomNumber);
        }
        public async Task<bool> AllRoomsExistAsync(List<int> roomIds)
        {
            var distinctIds = roomIds.Distinct().ToList();

            if (distinctIds.Count == 0)
                return false;

            int existingCount = await _context.Rooms.CountAsync(r => distinctIds.Contains(r.RoomId));

            return roomIds.Count == existingCount;
        }



        public async Task<List<Room>> GetRoomsByIdsAsync(List<int> roomIds)
        {
            return await _context.Rooms.AsNoTracking().Include(r => r.RoomType).
                Where(r => roomIds.Contains(r.RoomId)).ToListAsync();
        }


        //Rooms Availablity
        public async Task<List<Room>> GetAvailableRoomsAsync(List<int> bookedRoomIds)
        {
            return await _context.Rooms.AsNoTracking().Include(r => r.RoomType).Where(r => !bookedRoomIds.Contains(r.RoomId)).ToListAsync();
        }

        public async Task<List<decimal>> GetRoomPricesAsync(
    List<int> roomIds)
        {
            return await _context.Rooms
                .AsNoTracking()
                .Where(r => roomIds.Contains(r.RoomId))
                .Select(r => r.RoomType.PricePerNight)
                .ToListAsync();
        }
        public async Task<decimal> GetRoomPriceByRoomIdAsync(int roomId)
        {
            return await _context.Rooms
                .AsNoTracking()
                .Where(r => roomId == r.RoomId)
                .Select(r => r.RoomType.PricePerNight)
                .FirstOrDefaultAsync();
        }



    }
}
