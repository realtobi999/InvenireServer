
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Employees.Queries.GetByJwt;

public class GetByJwtEmployeeQueryHandler : IRequestHandler<GetByJwtEmployeeQuery, GetByJwtEmployeeQueryResponse>
{
    private readonly IServiceManager _services;

    public GetByJwtEmployeeQueryHandler(IServiceManager services)
    {
        _services = services;
    }

    public async Task<GetByJwtEmployeeQueryResponse> Handle(GetByJwtEmployeeQuery request, CancellationToken ct)
    {
        var employee = await _services.Employees.GetAsync(request.Jwt);

        return new GetByJwtEmployeeQueryResponse
        {
            EmployeeDto = new EmployeeDto
            {
                Id = employee.Id,
                OrganizationId = employee.OrganizationId,
                Name = employee.Name,
                EmailAddress = employee.EmailAddress,
                CreatedAt = employee.CreatedAt,
                LastUpdatedAt = employee.LastUpdatedAt,
                AssignedItems = [.. employee.AssignedItems.Select(i => new PropertyItemDto
                {
                    Id = i.Id,
                    PropertyId = i.PropertyId,
                    EmployeeId = i.EmployeeId,
                    InventoryNumber = i.InventoryNumber,
                    RegistrationNumber = i.RegistrationNumber,
                    Name = i.Name,
                    Price = i.Price,
                    SerialNumber = i.SerialNumber,
                    DateOfPurchase = i.DateOfPurchase,
                    DateOfSale = i.DateOfSale,
                    Location = new PropertyItemLocationDto
                    {
                        Room = i.Location.Room,
                        Building = i.Location.Building,
                        AdditionalNote = i.Location.AdditionalNote
                    },
                    Description = i.Description,
                    DocumentNumber = i.DocumentNumber,
                    CreatedAt = i.CreatedAt,
                    LastUpdatedAt = i.LastUpdatedAt
                })],
                Suggestions = [.. employee.Suggestions.Select(s => new PropertySuggestionDto
                {
                    Id = s.Id,
                    PropertyId = s.PropertyId,
                    EmployeeId = s.EmployeeId,
                    Name = s.Name,
                    Description = s.Description,
                    Feedback = s.Feedback,
                    PayloadString = s.PayloadString,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    ResolvedAt = s.ResolvedAt,
                    LastUpdatedAt = s.LastUpdatedAt
                })]
            }
        };
    }
}
