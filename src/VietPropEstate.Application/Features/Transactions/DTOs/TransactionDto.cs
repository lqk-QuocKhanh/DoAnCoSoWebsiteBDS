using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Transactions.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string? PropertyTitle { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid AgentId { get; set; }
    public string? AgentName { get; set; }
    public ListingType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public TransactionStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ContractNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}
