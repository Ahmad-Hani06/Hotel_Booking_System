using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.DataAccess
{
    public class ReservationData
    {
        public readonly HotelBookingDbContext _context;
        public readonly RoomData _roomData;
        public ReservationData(HotelBookingDbContext context, RoomData roomData)
        {
            _context = context;
            _roomData = roomData;
        }

        public async Task<bool> IsRoomBookedAsync(int reservationId, List<int> roomIds, DateOnly checkIn, DateOnly checkout)
        {
            return await _context.ReservationRooms.AnyAsync(r => roomIds.Contains(r.RoomId) && checkout > r.Reservation.CheckInDate && checkIn < r.Reservation.CheckOutDate && r.Reservation.ReservationStatus != "Cancelled" && reservationId != r.ReservationId);

        }
        public async Task<bool> IsRoomBookedAsync(List<int> roomIds, DateOnly checkIn, DateOnly checkout)
        {
            return await _context.ReservationRooms.AnyAsync(r => roomIds.Contains(r.RoomId) && checkout > r.Reservation.CheckInDate && checkIn <
            r.Reservation.CheckOutDate && r.Reservation.ReservationStatus != "Cancelled");
        }

        public async Task<int> AddReservationAsync(Reservation reservation)
        {
            ArgumentNullException.ThrowIfNull(reservation);
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return reservation.ReservationId;
        }
        public async Task<List<int>> BookedRoomsAsync(DateOnly checkInDate, DateOnly checkOutDate)
        {
            var bookedRoomIds = await _context.ReservationRooms.Where(rr => rr.Reservation.CheckInDate < checkOutDate &&
            rr.Reservation.CheckOutDate > checkInDate && rr.Reservation.ReservationStatus == "Confirmed").Select(rr => rr.RoomId).Distinct().ToListAsync();

            return bookedRoomIds;
        }

        public async Task<string?> GetReservationStatusAsync(int reservationId)
        {
            return await _context.Reservations.Where(r => r.ReservationId == reservationId).Select(r => r.ReservationStatus).FirstOrDefaultAsync();
        }
        public async Task<bool> CancelledReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations.Where(r => r.ReservationId == reservationId && (r.ReservationStatus == "Confirmed" || r.ReservationStatus == "Pending")).FirstOrDefaultAsync();

            if (reservation == null)
                return false;

            reservation.ReservationStatus = "Cancelled";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Reservation?> GetReservationDetailsByReservationIdAsync(int reservationId)
        {
            return await _context.Reservations
                .AsNoTracking()
                .Include(r => r.ReservationRooms)
                    .ThenInclude(rr => rr.Room)
                        .ThenInclude(room => room.RoomType)
                .Include(r => r.Guest)
                    .ThenInclude(g => g.Person)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);
        }

        public async Task<Reservation?> GetReservationForUpdateAsync(
    int reservationId)
        {
            return await _context.Reservations
                .Include(r => r.ReservationRooms)
                .FirstOrDefaultAsync(r =>
                    r.ReservationId == reservationId);
        }
        public async Task<bool> UpdateReservationByReservationIdAsync(int reservationId, Reservation reservation, List<int> newRoomIds, int userId)
        {
            var existingReservation = await _context.Reservations.Include(r => r.ReservationRooms).Where(r => r.ReservationId == reservationId).FirstOrDefaultAsync();
            if (existingReservation == null)
                return false;

            existingReservation.CheckInDate = reservation.CheckInDate;
            existingReservation.CheckOutDate = reservation.CheckOutDate;
            existingReservation.ReservationStatus = reservation.ReservationStatus;
            existingReservation.TotalAmount = reservation.TotalAmount;

            foreach (var rr in existingReservation.ReservationRooms)    
            {
                if (!newRoomIds.Contains(rr.RoomId))
                    _context.ReservationRooms.Remove(rr);
            }

            foreach (var roomId in newRoomIds.Distinct())
            {
                if (!existingReservation.ReservationRooms
                    .Any(rr => rr.RoomId == roomId))
                {
                    _context.ReservationRooms.Add(new ReservationRoom
                    {
                        ReservationId = reservationId,
                        RoomId = roomId,
                        PricePerNightAtBooking = await _roomData.GetRoomPriceByRoomIdAsync(roomId),
                        CreatedByUserId = userId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckInReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations.Where(r => r.ReservationId == reservationId && (r.ReservationStatus == "Confirmed" || r.ReservationStatus == "Pending")).FirstOrDefaultAsync();
            if (reservation == null)
                return false;

            reservation.ReservationStatus = "CheckedIn";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckOutReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations.Where(r => r.ReservationId == reservationId && (r.ReservationStatus == "CheckedIn")).FirstOrDefaultAsync();
            if (reservation == null)
                return false;

            reservation.ReservationStatus = "CheckedOut";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsReservationExistsByIdAsync(int reservationId)
        {
            return await _context.Reservations.AsNoTracking().AnyAsync(r => r.ReservationId == reservationId);
        }

        //public async Task<decimal> GetReservationTotalAmountByIdAsync(int reservationId)
        //{
        //    return await _context.Reservations.Where(r => r.ReservationId == reservationId).
        //        Select(r => (decimal?)r.TotalAmount ?? 0).FirstOrDefaultAsync();
        //}

    }
}
