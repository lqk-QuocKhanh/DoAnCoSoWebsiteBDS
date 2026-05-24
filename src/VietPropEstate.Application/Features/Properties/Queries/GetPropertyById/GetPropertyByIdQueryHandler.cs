using AutoMapper;
using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPropertyById;

public sealed class GetPropertyByIdQueryHandler
    : IRequestHandler<GetPropertyByIdQuery, PropertyDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetPropertyByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<PropertyDetailDto> Handle(
        GetPropertyByIdQuery request,
        CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdWithDetailsAsync(
            request.Id, asNoTracking: true, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.Id);

        EnsureCanViewProperty(property.Status, property.Agent?.UserId);

        await _unitOfWork.Properties.IncrementViewCountAsync(property.Id, cancellationToken);

        var dto = _mapper.Map<PropertyDetailDto>(property);

        if (property.PropertyType is not null)
            dto.PropertyTypeName = property.PropertyType.Name;

        if (property.Agent is not null)
        {
            dto.AgentName = property.Agent.FullName;
            dto.AgentPhone = property.Agent.PhoneNumber;
            dto.AgentEmail = property.Agent.Email;
            dto.AgentUserId = property.Agent.UserId;
        }

        return dto;
    }

    private void EnsureCanViewProperty(PropertyStatus status, string? agentUserId)
    {
        if (status == PropertyStatus.Active)
            return;

        if (_currentUser.IsInRole("Admin") || _currentUser.IsInRole("Staff"))
            return;

        if (!string.IsNullOrWhiteSpace(_currentUser.UserId) &&
            string.Equals(agentUserId, _currentUser.UserId, StringComparison.Ordinal))
            return;

        throw new NotFoundException(nameof(Domain.Entities.Property), "hidden");
    }
}
