using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Chat.Commands.StartConversation;
using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Chat.Commands.StartSupportConversation;

public sealed class StartSupportConversationCommandHandler
    : IRequestHandler<StartSupportConversationCommand, ConversationDto>
{
    private const string DefaultAdminEmail = "admin@vietpropestate.vn";

    private readonly IApplicationDbContext _db;
    private readonly IIdentityUserLookup _userLookup;
    private readonly IMediator _mediator;

    public StartSupportConversationCommandHandler(
        IApplicationDbContext db,
        IIdentityUserLookup userLookup,
        IMediator mediator)
    {
        _db = db;
        _userLookup = userLookup;
        _mediator = mediator;
    }

    public async Task<ConversationDto> Handle(
        StartSupportConversationCommand request,
        CancellationToken cancellationToken)
    {
        var adminEmail = string.IsNullOrWhiteSpace(request.AdminEmail)
            ? DefaultAdminEmail
            : request.AdminEmail.Trim();

        var adminId = await _userLookup.FindUserIdByEmailAsync(adminEmail, cancellationToken)
            ?? throw new InvalidOperationException("Không tìm thấy tài khoản Admin hỗ trợ.");

        if (adminId == request.UserId)
            throw new InvalidOperationException("Tài khoản Admin không thể mở hỗ trợ với chính mình.");

        var propertyId = await _db.Properties
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == PropertyStatus.Active)
            .OrderBy(p => p.CreatedAt)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (propertyId == Guid.Empty)
            throw new NotFoundException(nameof(Property), "active-property");

        return await _mediator.Send(new StartConversationCommand
        {
            PropertyId = propertyId,
            BuyerId = request.UserId,
            SellerId = adminId,
            Subject = "Hỗ trợ VietPropEstate",
            InitialMessage = request.InitialMessage
                ?? "Xin chào, tôi cần hỗ trợ từ Admin VietPropEstate."
        }, cancellationToken);
    }
}
