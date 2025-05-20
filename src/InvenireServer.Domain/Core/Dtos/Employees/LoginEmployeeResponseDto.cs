namespace InvenireServer.Domain.Core.Dtos.Employees;

public record class LoginEmployeeResponseDto
{
    public required string Token { get; set; }
}
