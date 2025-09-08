using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Common;

public class PaginationOptions
{
    public const int MAX_LIMIT = 100;

    public PaginationOptions(int? limit, int? offset)
    {
        if (limit is null)
            throw new BadRequest400Exception("The limit must be set.");
        if (offset is null)
            throw new BadRequest400Exception("The offset must be set.");

        Validate(limit.Value, offset.Value);

        Limit = limit.Value;
        Offset = offset.Value;
    }

    public PaginationOptions(int limit, int offset)
    {
        Validate(limit, offset);

        Limit = limit;
        Offset = offset;
    }

    public int Limit { get; }
    public int Offset { get; }

    private static void Validate(int limit, int offset)
    {
        if (limit <= 0)
            throw new BadRequest400Exception("The limit must be greater than zero.");
        if (limit > MAX_LIMIT)
            throw new BadRequest400Exception($"The limit for the query exceeds the maximum amount - {MAX_LIMIT}.");
        if (offset < 0)
            throw new BadRequest400Exception("The offset cannot be negative.");
    }
}
