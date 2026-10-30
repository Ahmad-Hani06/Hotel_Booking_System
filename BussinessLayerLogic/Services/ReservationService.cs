using BussinessLayerLogic.DTOs;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayerLogic.Services
{
    public class ReservationService
    {

        private readonly ReservationData _reservationData;
        private readonly ReservationRoomData _reservationRoomData;
        private readonly RoomData _roomData;
        private readonly GuestData _guestData;
        private readonly PersonData _personData;
        private readonly ReservationChargeData _reservationChargeData;
        private readonly PaymentData _paymentData;
        public ReservationService(ReservationData reservationData, RoomData roomData, 
            GuestData guestData, PersonData personData, 
            ReservationRoomData reservationRoomData, ReservationChargeData reservationChargeData, PaymentData paymentData)
        {
            _reservationData = reservationData;
            _roomData = roomData;
            _guestData = guestData;
            _personData = personData;
            _reservationRoomData = reservationRoomData;
            _reservationChargeData = reservationChargeData;
            _paymentData = paymentData;
        }

        private bool IsInvalidReservationDates(DateOnly checkIn, DateOnly checkOut)
        {
            return (checkIn >= checkOut || checkIn < DateOnly.FromDateTime(DateTime.Today));
        }
        public async Task<int> AddReservation(AddReservationDto reservationDto)
        {
            if (reservationDto.roomIds == null || reservationDto.roomIds.Count == 0)
            {
                throw new ArgumentException("At least one room is required.");
            }

            if (IsInvalidReservationDates(reservationDto.CheckInDate, reservationDto.CheckOutDate))
                throw new ArgumentException("Check-out date must be after check-in date.", nameof(reservationDto.CheckOutDate));

            if (await _reservationData.IsRoomBookedAsync(reservationDto.roomIds, reservationDto.CheckInDate, reservationDto.CheckOutDate))
                throw new InvalidOperationException("One or more selected rooms are already booked.");

            if (!await _roomData.AllRoomsExistAsync(reservationDto.roomIds))
            {
                throw new ArgumentException("One or more selected rooms do not exist.", nameof(reservationDto.roomIds));
            }

            using var transaction = await _reservationData._context.Database.BeginTransactionAsync();

            try
            {
                Person person = new Person
                {
                    FirstName = reservationDto.FirstName,
                    LastName = reservationDto.LastName,
                    Email = reservationDto.Email,
                    PhoneNumber = reservationDto.PhoneNumber
                };

                int personId = await _personData.AddPersonAsync(person);

                Guest guest = new Guest
                {
                    PersonId = personId,
                    RegistrationDate = DateTime.Now,
                    Notes = "Add New Guest"
                };

                int guestId = await _guestData.AddGuestAsync(guest);
                var selectedRooms = await _roomData.GetRoomsByIdsAsync(reservationDto.roomIds);

                Reservation reservation = new Reservation
                {
                    GuestId = guestId,
                    ReservationDate = DateTime.Now,
                    CheckInDate = reservationDto.CheckInDate,
                    CheckOutDate = reservationDto.CheckOutDate,
                    TotalAmount = (reservationDto.CheckOutDate.DayNumber - reservationDto.CheckInDate.DayNumber) * selectedRooms.Sum(room => room.RoomType.PricePerNight),
                    ReservationStatus = "Confirmed"
                }; 

                int reservationId = await _reservationData.AddReservationAsync(reservation);


                List<ReservationRoom> reservationRooms = new();

                foreach (var room in selectedRooms)    
                {
                    ReservationRoom reservationRoom = new ReservationRoom
                    {
                        ReservationId = reservationId,
                        RoomId = room.RoomId,
                        CreatedByUserId = reservationDto.UserId,
                        PricePerNightAtBooking = room.RoomType.PricePerNight
                    };

                    reservationRooms.Add(reservationRoom);
                }

                await _reservationRoomData.AddReservationRoomsAsync(reservationRooms);

                await transaction.CommitAsync();

                return reservationId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }



        }
        public async Task<bool> CancelledReservationAsync(int reservationId)
        {
            if (reservationId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(reservationId),
                    "Reservation ID must be greater than zero.");

            var status = await _reservationData.GetReservationStatusAsync(reservationId);
            if (status == null)
                throw new KeyNotFoundException(
                    "Reservation not found.");

            if (status == "Cancelled")
                throw new InvalidOperationException(
                    "Reservation is already cancelled.");

            if (status != "Confirmed" && status != "Pending")
                throw new InvalidOperationException(
                    "This reservation cannot be cancelled.");

            bool isCancelled = await _reservationData.CancelledReservationAsync(reservationId);


            if (!isCancelled)   
                throw new InvalidOperationException(
                    "Reservation status changed. Please retry.");

            return true;
        }
        public async Task<ReservationDetailsDto?> GetReservationDetailsByReservationIdAsync(int reservationId)
        {
            if (reservationId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(reservationId),
                    "Reservation ID must be greater than zero.");

            var reservation = await _reservationData.GetReservationDetailsByReservationIdAsync(reservationId);

            if (reservation == null)
                return null;

            return new ReservationDetailsDto
            {
                ReservationId = reservation.ReservationId,

                GuestFullName = reservation.Guest.Person.FirstName
                        + " " + reservation.Guest.Person.LastName,

                PhoneNumber = reservation.Guest.Person.PhoneNumber,
                Email = reservation.Guest.Person.Email,

                ReservationDate = reservation.ReservationDate,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                ReservationStatus = reservation.ReservationStatus,
                TotalAmount = reservation.TotalAmount,
                Rooms = reservation.ReservationRooms.Select(rr => new RoomResponseDto
                {
                    RoomId = rr.RoomId,
                    RoomNumber = rr.Room.RoomNumber,
                    FloorNumber = rr.Room.FloorNumber,
                    RoomTypeName = rr.Room.RoomType.TypeName,
                    PricePerNight = rr.Room.RoomType.PricePerNight
                }).ToList()
            };
        }

        public async Task<bool> UpdateReservationByReservationIdAsync(int reservationId, UpdateReservationDto reservationDto)
        {
            // 1. Check if reservation exists
            var reservation = await _reservationData.GetReservationForUpdateAsync(reservationId);

            if (reservation == null)
                return false;

            if (IsInvalidReservationDates( reservationDto.CheckInDate, reservationDto.CheckOutDate))
            {
                throw new ArgumentException(
                    "Check-out date must be after check-in date.");
            }

          
            if (!await _roomData.AllRoomsExistAsync(reservationDto.RoomIds))
            {
                throw new ArgumentException(
                    "One or more selected rooms do not exist.");
            }


            if (await _reservationData.IsRoomBookedAsync(reservationId ,reservationDto.RoomIds,reservationDto.CheckInDate,reservationDto.CheckOutDate))
            {
                throw new InvalidOperationException(
                    "One or more selected rooms are already booked.");
            }

            decimal totalAmount = await CalculateTotalAmountAsync(
                 reservationDto.RoomIds,
                 reservationDto.CheckInDate,
                 reservationDto.CheckOutDate);

            // 9. Prepare updated reservation
            reservation.CheckInDate = reservationDto.CheckInDate;
            reservation.ReservationStatus = reservationDto.ReservationStatus;
            reservation.CheckOutDate = reservationDto.CheckOutDate;
            reservation.TotalAmount = totalAmount;

            // 10. Update reservation and rooms
            return await _reservationData.UpdateReservationByReservationIdAsync(
                    reservationId,
                    reservation,
                    reservationDto.RoomIds, reservationDto.CreatedByUserId);
        }

        private async Task<decimal> CalculateTotalAmountAsync(
    List<int> roomIds,
    DateOnly checkInDate,
    DateOnly checkOutDate)
        {
            int numberOfNights =
                checkOutDate.DayNumber - checkInDate.DayNumber;

            var roomPrices = await _roomData.GetRoomPricesAsync(roomIds);

            decimal totalAmount = roomPrices.Sum() * numberOfNights;

            return totalAmount;
        }

        public async Task<bool> CheckInReservationAsync(int reservationId)
        {

            var reservation = await _reservationData.GetReservationDetailsByReservationIdAsync(reservationId);
            if (reservation == null)
                return false;

            if (reservation.ReservationStatus != "Confirmed" && reservation.ReservationStatus != "Pending")
                throw new InvalidOperationException(
                    "Only confirmed reservations can be checked in.");

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (today < reservation.CheckInDate)
                throw new InvalidOperationException("Check-in date has not arrived yet.");

            if (today >= reservation.CheckOutDate)
                throw new InvalidOperationException(
                    "Reservation has already expired.");

            return await _reservationData.CheckInReservationAsync(reservationId);
        }

        public async Task<bool> CheckOutReservationAsync(int reservationId)
        {

            var reservation = await _reservationData.GetReservationDetailsByReservationIdAsync(reservationId);
            if (reservation == null)
                return false;

            if (reservation.ReservationStatus != "CheckedIn")
                throw new InvalidOperationException("Only CheckedIn reservations can be checked out.");

            decimal totalAmount = reservation.TotalAmount;

            totalAmount += await _reservationChargeData.GetReservationChargeAmountByReservationIdAsync(reservationId);

            // Payments
            decimal paymentAmount = await _paymentData.GetTotalPaymentsByReservationIdAsync(reservationId);

            decimal pendingBalance = totalAmount - paymentAmount;
            if (paymentAmount < totalAmount)
                throw new InvalidOperationException($"Cannot check out. Pending balance: {pendingBalance:C}. Please settle the payment first.");

            return await _reservationData.CheckOutReservationAsync(reservationId);
        }

    }
}
