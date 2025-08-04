using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Common;

public class PaginationParameters
{
    public const int MAX_LIMIT = 100;

    public PaginationParameters(int limit, int offset)
    {
        if (limit <= 0)
            throw new BadRequest400Exception("Limit must be greater than zero.");

        if (limit > MAX_LIMIT)
            throw new BadRequest400Exception($"The limit for the query exceeds the maximum amount - {MAX_LIMIT}.");

        if (offset < 0)
            throw new BadRequest400Exception("Offset cannot be negative.");

        Limit = limit;
        Offset = offset;
    }

    public int Limit { get; }
    public int Offset { get; }
}
