using AutoMapper;
using MediatR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPropertyBySlug;

public sealed class GetPropertyBySlugQueryHandler : IRequestHandler<GetPropertyBySlugQuery, PropertyDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetPropertyBySlugQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<PropertyDetailDto> Handle(GetPropertyBySlugQuery request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetBySlugAsync(request.Slug, cancellationToken)
            ?? throw new NotFoundException($"Property with slug '{request.Slug}' was not found.");

        EnsureCanViewProperty(property.Status, property.Agent?.UserId);

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
