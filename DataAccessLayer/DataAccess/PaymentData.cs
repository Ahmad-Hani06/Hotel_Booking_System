using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.DataAccess
{
    public class PaymentData
    {
        private readonly HotelBookingDbContext _context;

        public PaymentData(HotelBookingDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalPaymentsByReservationIdAsync(int reservationId)
        {

            return await _context.Payments.Where(r => r.ReservationId == reservationId).
                SumAsync(r => (decimal?)r.Amount ?? 0);
        }
    }
}
