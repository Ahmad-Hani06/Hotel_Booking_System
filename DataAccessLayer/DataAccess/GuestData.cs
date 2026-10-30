using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DataAccess;

public class GuestData
{
    private readonly HotelBookingDbContext _context;

    public GuestData(HotelBookingDbContext context)
    {
        _context = context;
    }

    public Task<List<Guest>> GetAllGuestsAsync()
    {
        return _context.Guests.AsNoTracking().Include(g => g.Person).OrderBy(g => g.Person.FirstName).ThenBy(g => g.Person.LastName).ToListAsync();
    }

    public Task<Guest?> GetGuestByIdAsync(int guestId)
    {
        return _context.Guests
            .AsNoTracking()
            .Include(g => g.Person)
            .FirstOrDefaultAsync(g => g.GuestId == guestId);
    }

    public Task<List<Guest>> SearchGuestsAsync(string searchTerm)
    {
        return _context.Guests
            .AsNoTracking()
            .Include(g => g.Person)
            .Where(g => g.Person.FirstName.Contains(searchTerm)
                || g.Person.LastName.Contains(searchTerm)
                || (g.Person.FirstName + " " + g.Person.LastName).Contains(searchTerm)
                || g.Person.PhoneNumber.Contains(searchTerm)
                || (g.Person.Email != null && g.Person.Email.Contains(searchTerm)))
            .OrderBy(g => g.Person.FirstName)
            .ThenBy(g => g.Person.LastName)
            .ToListAsync();
    }

    public Task<bool> IsDuplicateAsync(string phoneNumber, string? email)
    {
        return _context.Guests.AnyAsync(g =>
            g.Person.PhoneNumber == phoneNumber ||
            (email != null && g.Person.Email == email)
        );
    }

    public async Task<int> AddGuestAsync(Guest guest)
    {
        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();
        return guest.GuestId;
    }

    public async Task<bool> UpdateGuestAsync(int guestId, Person person, string? notes)
    {
        var guest = await _context.Guests
            .Include(g => g.Person)
            .FirstOrDefaultAsync(g => g.GuestId == guestId);

        if (guest == null)
            return false;

        guest.Person.FirstName = person.FirstName;
        guest.Person.LastName = person.LastName;
        guest.Person.PhoneNumber = person.PhoneNumber;
        guest.Person.Email = person.Email;
        guest.Notes = notes;

        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> HasReservationsAsync(int guestId)
    {
        return _context.Reservations
            .AsNoTracking()
            .AnyAsync(r => r.GuestId == guestId);
    }

    public async Task<bool> DeleteGuestAsync(int guestId)
    {
        var guest = await _context.Guests
            .Include(g => g.Person)
            .FirstOrDefaultAsync(g => g.GuestId == guestId);

        if (guest == null)
            return false;

        bool personIsUser = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.PersonId == guest.PersonId);

        _context.Guests.Remove(guest);
        if (!personIsUser)
            _context.Persons.Remove(guest.Person);

        await _context.SaveChangesAsync();
        return true;
    }

    public Task<List<Reservation>> GetGuestReservationsAsync(int guestId)
    {
        return _context.Reservations
            .AsNoTracking()
            .Where(r => r.GuestId == guestId)
            .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();
    }

    public async Task<int?> GetGuestIdByName(string FirstName, string LastName)
    {
        return await _context.Guests.AsNoTracking().Where(g => g.Person.FirstName == FirstName && g.Person.LastName == LastName).Select(g => (int?)g.GuestId).FirstOrDefaultAsync();
    }
}
