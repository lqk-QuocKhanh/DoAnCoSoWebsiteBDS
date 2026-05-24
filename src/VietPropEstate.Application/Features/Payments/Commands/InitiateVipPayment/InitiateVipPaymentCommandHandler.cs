using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Authorization;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Payments.DTOs;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.ValueObjects;

namespace VietPropEstate.Application.Features.Payments.Commands.InitiateVipPayment;

public sealed class InitiateVipPaymentCommandHandler
    : IRequestHandler<InitiateVipPaymentCommand, PaymentInitiationDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IVnPayService _vnPay;
    private readonly ICurrentUserService _currentUser;

    private const int ExpireMinutes = 15;

    public InitiateVipPaymentCommandHandler(
        IApplicationDbContext db,
        IVnPayService vnPay,
        ICurrentUserService currentUser)
    {
        _db = db;
        _vnPay = vnPay;
        _currentUser = currentUser;
    }

    public async Task<PaymentInitiationDto> Handle(
        InitiateVipPaymentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole(AppRoles.Broker))
            throw new ForbiddenAccessException("Chỉ tài khoản Môi giới mới được mua gói VIP.");
        var package = await _db.VIPPackages
            .FirstOrDefaultAsync(p => p.Id == request.VIPPackageId && p.IsActive && !p.IsDeleted,
                cancellationToken)
            ?? throw new NotFoundException(nameof(VIPPackage), request.VIPPackageId);

        // Build a human-readable order info (max 255 chars for VNPay)
        var orderInfo = $"Thanh toan goi VIP {package.Name} - {package.DurationDays} ngay";

        // Payment reference = 32-char hex GUID (used as vnp_TxnRef)
        var paymentId = Guid.NewGuid();
        var paymentReference = paymentId.ToString("N"); // 32 hex chars, no dashes

        var payment = Payment.Create(
            userId: request.UserId,
            amount: new Money(package.Price.Amount, "VND"),
            paymentReference: paymentReference,
            paymentMethod: "VNPay",
            orderInfo: orderInfo,
            ipAddress: request.IpAddress,
            vipPackageId: package.Id);

        // Override the auto-generated ID so it matches our reference
        // (We already set paymentId above — EF uses the entity's Id property)
        await _db.Payments.AddAsync(payment, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        var paymentUrl = _vnPay.UseMockPayment
            ? _vnPay.CreateMockPaymentUrl(paymentReference, package.Price.Amount, package.Name)
            : _vnPay.CreatePaymentUrl(
                txnRef: paymentReference,
                amountVnd: package.Price.Amount,
                orderInfo: orderInfo,
                ipAddress: request.IpAddress,
                locale: request.Locale,
                expireMinutes: ExpireMinutes);

        return new PaymentInitiationDto
        {
            PaymentId = payment.Id,
            PaymentReference = paymentReference,
            PaymentUrl = paymentUrl,
            Amount = package.Price.Amount,
            Currency = "VND",
            VIPPackageName = package.Name,
            ExpiresInMinutes = ExpireMinutes
        };
    }
}
