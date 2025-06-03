namespace InvenireServer.Domain.Exceptions.Common;

/// <summary>
/// Exception thrown when a database operation completes without modifying any rows.
/// </summary>
public class ZeroRowsAffectedException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="ZeroRowsAffectedException"/> with a default error message indicating no rows were affected.
    /// </summary>
    public ZeroRowsAffectedException() : base("Database modification attempt resulted in zero rows being affected.")
    {
    }

    /// <inheritdoc/>
    public ZeroRowsAffectedException(string message) : base(message)
    {
    }
}