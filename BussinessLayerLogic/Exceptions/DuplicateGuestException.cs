namespace BussinessLayerLogic.Exceptions;

public class DuplicateGuestException : Exception
{
    public DuplicateGuestException()
        : base("A guest with the same email address or phone number already exists.")
    {
    }
}
