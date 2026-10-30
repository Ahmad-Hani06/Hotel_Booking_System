using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using BussinessLayerLogic.DTOs;
using BussinessLayerLogic.Exceptions;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Entities;

namespace BussinessLayerLogic.Services;

public class GuestService
{
    private static readonly Regex PhoneNumberRegex = new(
        @"^\+?[0-9\s\-()]{7,20}$",
        RegexOptions.Compiled);

    private readonly GuestData _guestData;

    public GuestService(GuestData guestData)
    {
        _guestData = guestData;
    }

    public async Task<List<GuestResponseDto>> GetAllGuestsAsync()
    {
        var guests = await _guestData.GetAllGuestsAsync();
        return guests.Select(MapGuest).ToList();
    }

    public async Task<GuestResponseDto?> GetGuestByIdAsync(int guestId)
    {
        ValidateGuestId(guestId);
        var guest = await _guestData.GetGuestByIdAsync(guestId);
        return guest == null ? null : MapGuest(guest);
    }

    public async Task<List<GuestResponseDto>> SearchGuestsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term is required.", nameof(searchTerm));

        var guests = await _guestData.SearchGuestsAsync(searchTerm.Trim());
        return guests.Select(MapGuest).ToList();
    }

    public async Task<GuestResponseDto> AddGuestAsync(AddGuestDto guestDto)
    {
        ArgumentNullException.ThrowIfNull(guestDto);

        if (await _guestData.IsDuplicateAsync(guestDto.PhoneNumber, guestDto.Email))
            throw new DuplicateGuestException();

        var guest = new Guest
        {
            Notes = guestDto.Notes,
            Person = new Person
            {
                FirstName = guestDto.FirstName,
                LastName = guestDto.LastName,
                PhoneNumber = guestDto.PhoneNumber,
                Email = guestDto.Email
            }
        };

        int guestId = await _guestData.AddGuestAsync(guest);
        var createdGuest = await _guestData.GetGuestByIdAsync(guestId);
        return MapGuest(createdGuest!); 
    }

    public async Task<bool> UpdateGuestAsync(int guestId, UpdateGuestDto guestDto)
    {
        ValidateGuestId(guestId);
        ArgumentNullException.ThrowIfNull(guestDto);

        if (await _guestData.IsDuplicateAsync(guestDto.PhoneNumber, guestDto.Email))
            throw new DuplicateGuestException();

        return await _guestData.UpdateGuestAsync(
            guestId,
            new Person
            {
                FirstName = guestDto.FirstName,
                LastName = guestDto.LastName,
                PhoneNumber = guestDto.PhoneNumber,
                Email = guestDto.Email
            },
            guestDto.Notes);
    }

    public async Task<bool> DeleteGuestAsync(int guestId)
    {
        ValidateGuestId(guestId);

        if (await _guestData.HasReservationsAsync(guestId))
            throw new GuestHasReservationsException(guestId);

        return await _guestData.DeleteGuestAsync(guestId);
    }

    //public async Task<List<GuestReservationResponseDto>?> GetGuestReservationsAsync(int guestId)
    //{
    //    ValidateGuestId(guestId);

    //    if (await _guestData.GetGuestByIdAsync(guestId) == null)
    //        return null;

    //    var reservations = await _guestData.GetGuestReservationsAsync(guestId);
    //    return reservations.Select(r => new GuestReservationResponseDto
    //    {
    //        ReservationId = r.ReservationId,
    //        ReservationDate = r.ReservationDate,
    //        CheckInDate = r.CheckInDate,
    //        CheckOutDate = r.CheckOutDate,
    //        TotalAmount = r.TotalAmount,
    //        ReservationStatus = r.ReservationStatus,
    //        RoomNumbers = r.ReservationRooms
    //            .Select(rr => rr.Room.RoomNumber)
    //            .OrderBy(number => number)
    //            .ToList()
    //    }).ToList();
    //}

    private static GuestResponseDto MapGuest(Guest guest)
    {
        return new GuestResponseDto
        {
            GuestId = guest.GuestId,
            PersonId = guest.PersonId,
            FirstName = guest.Person.FirstName,
            LastName = guest.Person.LastName,
            PhoneNumber = guest.Person.PhoneNumber,
            Email = guest.Person.Email,
            RegistrationDate = guest.RegistrationDate,
            Notes = guest.Notes
        };
    }

    private static void ValidateGuestId(int guestId)
    {
        if (guestId <= 0)
            throw new ArgumentOutOfRangeException(nameof(guestId), "Guest ID must be greater than zero.");
    }


    //private sealed record GuestValues(
    //    string FirstName,
    //    string LastName,
    //    string PhoneNumber,
    //    string? Email,
    //    string? Notes);
}
