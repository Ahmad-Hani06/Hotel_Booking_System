using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.DataAccess
{
    public class ReservationChargeData
    {
        private readonly HotelBookingDbContext _context;

        public ReservationChargeData(HotelBookingDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetReservationChargeAmountByReservationIdAsync(int reservationId)
        {
            return await _context.ReservationCharges.Where(r => r.ReservationId == reservationId).
                SumAsync(r => (decimal?)r.Amount ?? 0);
        }
    }
}
