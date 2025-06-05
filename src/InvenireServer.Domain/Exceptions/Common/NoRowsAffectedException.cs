namespace InvenireServer.Domain.Exceptions.Common;

/// <summary>
/// Exception thrown when a database operation completes without modifying any rows.
/// </summary>
public class NoRowsAffectedException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="NoRowsAffectedException"/> with a default error message indicating no rows were affected.
    /// </summary>
    public NoRowsAffectedException() : base("Database modification attempt resulted in zero rows being affected.")
    {
    }

    /// <inheritdoc/>
    public NoRowsAffectedException(string message) : base(message)
    {
    }
}