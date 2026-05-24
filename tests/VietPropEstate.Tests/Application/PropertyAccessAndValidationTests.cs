using AutoMapper;
using FluentAssertions;
using VietPropEstate.Application.Features.Properties.Commands.AddPropertyImage;
using VietPropEstate.Application.Features.Properties.Queries.GetPropertyBySlug;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Interfaces;
using Moq;
using VietPropEstate.Tests.Helpers;

namespace VietPropEstate.Tests.Application;

public class PropertyAccessAndValidationTests
{
    [Theory]
    [InlineData("/uploads/properties/a.jpg", true)]
    [InlineData("https://cdn.example.com/a.jpg", true)]
    [InlineData("not-a-url", false)]
    [InlineData("", false)]
    public void AddPropertyImageValidator_AcceptsExpectedUrlFormats(string url, bool isValid)
    {
        var validator = new AddPropertyImageCommandValidator();
        var command = new AddPropertyImageCommand
        {
            PropertyId = Guid.NewGuid(),
            Url = url
        };

        var result = validator.Validate(command);

        result.IsValid.Should().Be(isValid);
        if (!isValid)
            result.Errors.Should().Contain(e => e.PropertyName == nameof(AddPropertyImageCommand.Url));
    }

    [Fact]
    public async Task GetPropertyBySlug_ActiveListing_ReturnsDtoForAnonymousUser()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.Active);
        var handler = BuildSlugHandler(property, isAdmin: false, userId: null);

        var dto = await handler.Handle(new GetPropertyBySlugQuery(property.Slug), CancellationToken.None);

        dto.Id.Should().Be(property.Id);
        dto.Title.Should().Be(property.Title);
    }

    [Fact]
    public async Task GetPropertyBySlug_PendingListing_HiddenFromAnonymousUser()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.PendingApproval);
        var handler = BuildSlugHandler(property, isAdmin: false, userId: null);

        var act = () => handler.Handle(new GetPropertyBySlugQuery(property.Slug), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetPropertyBySlug_PendingListing_VisibleToOwner()
    {
        const string ownerUserId = "owner-user-id";
        var agent = PropertyTestFactory.CreateAgent(ownerUserId);
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.PendingApproval, agentId: agent.Id);
        PropertyTestFactory.AttachAgent(property, agent);

        var handler = BuildSlugHandler(property, isAdmin: false, userId: ownerUserId);

        var dto = await handler.Handle(new GetPropertyBySlugQuery(property.Slug), CancellationToken.None);

        dto.AgentUserId.Should().Be(ownerUserId);
    }

    [Fact]
    public async Task GetPropertyBySlug_PendingListing_VisibleToAdmin()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.PendingApproval);
        var handler = BuildSlugHandler(property, isAdmin: true, userId: "admin-user");

        var dto = await handler.Handle(new GetPropertyBySlugQuery(property.Slug), CancellationToken.None);

        dto.Id.Should().Be(property.Id);
    }

    private static GetPropertyBySlugQueryHandler BuildSlugHandler(
        VietPropEstate.Domain.Entities.Property property,
        bool isAdmin,
        string? userId)
    {
        var propertyRepo = new Mock<IPropertyRepository>();
        propertyRepo.Setup(r => r.GetBySlugAsync(property.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Properties).Returns(propertyRepo.Object);

        var mapper = new Mock<IMapper>();
        mapper.Setup(m => m.Map<PropertyDetailDto>(property))
            .Returns(new PropertyDetailDto
            {
                Id = property.Id,
                Title = property.Title,
                Slug = property.Slug,
                AgentUserId = property.Agent?.UserId
            });

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(u => u.UserId).Returns(userId);
        currentUser.Setup(u => u.IsInRole("Admin")).Returns(isAdmin);
        currentUser.Setup(u => u.IsInRole("Staff")).Returns(false);

        return new GetPropertyBySlugQueryHandler(unitOfWork.Object, mapper.Object, currentUser.Object);
    }
}
