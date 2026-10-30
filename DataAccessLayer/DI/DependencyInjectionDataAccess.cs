using DataAccessLayer.Data;
using DataAccessLayer.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer.DI;

public static class DependencyInjectionDataAccess
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<HotelBookingDbContext>(
            options => options.UseSqlServer(connectionString));

        services.AddScoped<RoomTypeData>();
        services.AddScoped<RoomData>();
        services.AddScoped<GuestData>();
        services.AddScoped<PersonData>();
        services.AddScoped<ReservationRoomData>();


        return services;
    }
}
