using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DataAccess;

public class ReservationRoomData
{
    private readonly HotelBookingDbContext _context;

    public ReservationRoomData(HotelBookingDbContext context)
    {
        _context = context;
    }

    public async Task AddReservationRoomsAsync(List<ReservationRoom> reservationRooms)
    {
        ArgumentNullException.ThrowIfNull(reservationRooms);

        if (reservationRooms.Count == 0)
            throw new ArgumentException("At least one reservation room is required.", nameof(reservationRooms));

        _context.ReservationRooms.AddRange(reservationRooms);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> SumPricesPerNightBookingByRoomIdsAsync(List<int>roomIds)
    {
        var roomPerNightPrices = await _context.ReservationRooms.AsNoTracking().
            Where(r => roomIds.Contains(r.RoomId)).SumAsync(r => r.PricePerNightAtBooking);

        return roomPerNightPrices;
    }


}
