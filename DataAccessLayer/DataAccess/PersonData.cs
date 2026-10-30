using DataAccessLayer.Data;
using DataAccessLayer.Entities;

namespace DataAccessLayer.DataAccess;

public class PersonData
{
    private readonly HotelBookingDbContext _context;

    public PersonData(HotelBookingDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddPersonAsync(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        _context.Persons.Add(person);
        await _context.SaveChangesAsync();

        return person.PersonId;
    }
}
