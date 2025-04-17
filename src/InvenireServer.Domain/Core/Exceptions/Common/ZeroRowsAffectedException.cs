namespace InvenireServer.Domain.Core.Exceptions.Common;

public class ZeroRowsAffectedException : Exception
{
    public ZeroRowsAffectedException(string message) : base(message)
    {
    }

    public ZeroRowsAffectedException() : base("Database modification attempt resulted in zero rows being affected.")
    {
    }
}