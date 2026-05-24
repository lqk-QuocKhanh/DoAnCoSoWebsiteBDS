using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Customers.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public CustomerType CustomerType { get; set; }
    public bool IsActive { get; set; }
}

public class CustomerDetailDto : CustomerDto
{
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TransactionCount { get; set; }
}
