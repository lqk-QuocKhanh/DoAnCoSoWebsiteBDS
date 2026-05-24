using MediatR;

namespace VietPropEstate.Application.Features.Payments.Commands.ProcessVnPayIpn;

/// <summary>
/// Handles VNPay's server-to-server IPN (Instant Payment Notification).
/// Returns a standardised IPN response that must be sent back to VNPay.
/// </summary>
public sealed record ProcessVnPayIpnCommand : IRequest<VnPayIpnResponse>
{
    /// <summary>All query/form parameters from the IPN request.</summary>
    public IEnumerable<KeyValuePair<string, string>> Parameters { get; init; } = [];
}

/// <summary>VNPay-compliant IPN response payload.</summary>
public sealed record VnPayIpnResponse(string RspCode, string Message);
