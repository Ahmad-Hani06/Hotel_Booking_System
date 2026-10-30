using BussinessLayerLogic.Services;
using DataAccessLayer.DataAccess;
using DataAccessLayer.DI;
using Microsoft.Extensions.DependencyInjection;

namespace BussinessLayerLogic.DI;

public static class DependencyInjectionBusiness
{
    public static IServiceCollection AddBusinessLayer(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDataAccessLayer(connectionString);

        services.AddScoped<RoomTypeService>();
        services.AddScoped<RoomService>();
        services.AddScoped<GuestService>();
        services.AddScoped<ReservationData>();
        services.AddScoped<ReservationService>();
        services.AddScoped<ReservationChargeData>();
        //services.AddScoped<ReservationCharge>();
        services.AddScoped<PaymentData>();

        return services;
    }
}
