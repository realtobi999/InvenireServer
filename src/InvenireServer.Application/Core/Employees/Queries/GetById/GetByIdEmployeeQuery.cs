namespace InvenireServer.Application.Core.Employees.Queries.GetById;

public record GetByIdEmployeeQuery : IRequest<GetByIdEmployeeQueryResponse>
{
    public required Guid EmployeeId { get; set; }
}
