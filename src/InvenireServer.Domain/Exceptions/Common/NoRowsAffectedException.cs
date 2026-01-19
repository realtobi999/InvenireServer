namespace InvenireServer.Domain.Exceptions.Common;

/// <summary>
/// Represents an exception thrown when a database modification affects no rows.
/// </summary>
public class NoRowsAffectedException : Exception
{
    public NoRowsAffectedException() : base("Database modification attempt resulted in zero rows being affected.")
    {
    }

    public NoRowsAffectedException(string message) : base(message)
    {
    }
}
