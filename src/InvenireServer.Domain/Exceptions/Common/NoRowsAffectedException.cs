namespace InvenireServer.Domain.Exceptions.Common;

public class NoRowsAffectedException : Exception
{
    public NoRowsAffectedException() : base("Database modification attempt resulted in zero rows being affected.")
    {
    }

    public NoRowsAffectedException(string message) : base(message)
    {
    }
}