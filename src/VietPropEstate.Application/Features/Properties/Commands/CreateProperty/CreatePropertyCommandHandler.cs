using MediatR;
using VietPropEstate.Application.Common.Authorization;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Interfaces;
using VietPropEstate.Domain.ValueObjects;

namespace VietPropEstate.Application.Features.Properties.Commands.CreateProperty;

public sealed class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAgentService _agentService;
    private readonly IUserPhoneVerificationService _phoneVerification;
    private readonly ICurrentUserService _currentUser;

    public CreatePropertyCommandHandler(
        IUnitOfWork unitOfWork,
        IAgentService agentService,
        IUserPhoneVerificationService phoneVerification,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _agentService = agentService;
        _phoneVerification = phoneVerification;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole(AppRoles.Customer) && !_currentUser.IsInRole(AppRoles.Broker))
            throw new ForbiddenAccessException("Tài khoản của bạn không được phép đăng tin.");

        await _phoneVerification.EnsureCanPostListingAsync(cancellationToken);

        if (!await _unitOfWork.PropertyTypes.ExistsAsync(
                pt => pt.Id == request.PropertyTypeId && !pt.IsDeleted, cancellationToken))
            throw new NotFoundException(nameof(PropertyType), request.PropertyTypeId);

        var agentId = request.AgentId != Guid.Empty
            ? request.AgentId
            : await _agentService.GetOrCreateAgentIdForCurrentUserAsync(cancellationToken);

        if (!await _unitOfWork.Agents.ExistsAsync(
                a => a.Id == agentId && a.IsActive && !a.IsDeleted, cancellationToken))
            throw new NotFoundException(nameof(Agent), agentId);

        var price = new Money(request.Price, request.Currency);
        var address = new Address(
            request.Street, request.Ward, request.District, request.Province,
            latitude: request.Latitude, longitude: request.Longitude);

        var property = Property.Create(
            request.Title,
            request.Description,
            price,
            request.Area,
            request.ListingType,
            address,
            request.PropertyTypeId,
            agentId,
            request.NumberOfBedrooms,
            request.NumberOfBathrooms,
            request.NumberOfFloors,
            request.Direction,
            request.TransactionTypeId,
            request.ProvinceCode,
            request.ProvinceName,
            request.WardCode,
            request.WardName,
            request.Latitude,
            request.Longitude);

        await _unitOfWork.Properties.AddAsync(property, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return property.Id;
    }
}
