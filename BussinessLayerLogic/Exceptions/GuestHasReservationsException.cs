namespace BussinessLayerLogic.Exceptions;

public class GuestHasReservationsException : Exception
{
    public GuestHasReservationsException(int guestId)
        : base($"Guest {guestId} cannot be deleted because they have reservations.")
    {
    }
}
