using MediatR;
using VietPropEstate.Application.Features.Payments.DTOs;

namespace VietPropEstate.Application.Features.Payments.Commands.InitiateVipPayment;

public sealed record InitiateVipPaymentCommand : IRequest<PaymentInitiationDto>
{
    public string UserId { get; init; } = string.Empty;
    public Guid VIPPackageId { get; init; }
    public string IpAddress { get; init; } = "127.0.0.1";
    public string Locale { get; init; } = "vn";
}
