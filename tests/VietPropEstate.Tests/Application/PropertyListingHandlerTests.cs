using FluentAssertions;
using Moq;
using VietPropEstate.Application.Common.Authorization;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Properties.Commands.AddPropertyImage;
using VietPropEstate.Application.Features.Properties.Commands.CreateProperty;
using VietPropEstate.Application.Features.Properties.Commands.PublishProperty;
using VietPropEstate.Application.Features.Properties.Commands.RejectProperty;
using VietPropEstate.Application.Features.Properties.Commands.SubmitProperty;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Interfaces;
using VietPropEstate.Tests.Helpers;

namespace VietPropEstate.Tests.Application;

public class PropertyListingHandlerTests
{
    [Fact]
    public async Task SubmitPropertyCommand_Owner_SubmitsDraftForApproval()
    {
        var agentId = Guid.NewGuid();
        var property = PropertyTestFactory.CreateProperty(agentId: agentId);
        var unitOfWork = CreateUnitOfWork(property);
        var agentService = new Mock<IAgentService>();
        agentService.Setup(s => s.GetOrCreateAgentIdForCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(agentId);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(u => u.IsInRole("Admin")).Returns(false);

        var handler = new SubmitPropertyCommandHandler(unitOfWork.Object, agentService.Object, currentUser.Object);

        await handler.Handle(new SubmitPropertyCommand(property.Id), CancellationToken.None);

        property.Status.Should().Be(PropertyStatus.PendingApproval);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SubmitPropertyCommand_NonOwner_ThrowsForbidden()
    {
        var property = PropertyTestFactory.CreateProperty(agentId: Guid.NewGuid());
        var unitOfWork = CreateUnitOfWork(property);
        var agentService = new Mock<IAgentService>();
        agentService.Setup(s => s.GetOrCreateAgentIdForCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(u => u.IsInRole("Admin")).Returns(false);

        var handler = new SubmitPropertyCommandHandler(unitOfWork.Object, agentService.Object, currentUser.Object);

        var act = () => handler.Handle(new SubmitPropertyCommand(property.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task PublishPropertyCommand_FromPendingApproval_SetsActive()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.PendingApproval);
        var unitOfWork = CreateUnitOfWork(property);
        var handler = new PublishPropertyCommandHandler(unitOfWork.Object);

        await handler.Handle(new PublishPropertyCommand(property.Id), CancellationToken.None);

        property.Status.Should().Be(PropertyStatus.Active);
    }

    [Fact]
    public async Task RejectPropertyCommand_FromPendingApproval_SetsRejected()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.PendingApproval);
        var unitOfWork = CreateUnitOfWork(property);
        var handler = new RejectPropertyCommandHandler(unitOfWork.Object);

        await handler.Handle(new RejectPropertyCommand(property.Id), CancellationToken.None);

        property.Status.Should().Be(PropertyStatus.Rejected);
    }

    [Fact]
    public async Task AddPropertyImageCommand_AddsFirstImageAsPrimary()
    {
        var property = PropertyTestFactory.CreateProperty();
        PropertyImage? captured = null;

        var propertyRepo = new Mock<IPropertyRepository>();
        propertyRepo.Setup(r => r.GetByIdWithDetailsAsync(property.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);
        propertyRepo.Setup(r => r.AddPropertyImageAsync(It.IsAny<PropertyImage>(), It.IsAny<CancellationToken>()))
            .Callback<PropertyImage, CancellationToken>((img, _) => captured = img)
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Properties).Returns(propertyRepo.Object);
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new AddPropertyImageCommandHandler(unitOfWork.Object);
        var imageId = await handler.Handle(new AddPropertyImageCommand
        {
            PropertyId = property.Id,
            Url = "/uploads/properties/test.jpg"
        }, CancellationToken.None);

        imageId.Should().NotBeEmpty();
        captured.Should().NotBeNull();
        captured!.IsPrimary.Should().BeTrue();
        captured.DisplayOrder.Should().Be(0);
    }

    [Fact]
    public async Task CreatePropertyCommand_ValidRequest_CreatesDraftProperty()
    {
        var agentId = Guid.NewGuid();
        var propertyTypeId = Guid.NewGuid();
        Property? created = null;

        var propertyRepo = new Mock<IPropertyRepository>();
        propertyRepo.Setup(r => r.AddAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()))
            .Callback<Property, CancellationToken>((p, _) => created = p)
            .Returns(Task.CompletedTask);

        var propertyTypes = new Mock<IRepository<PropertyType>>();
        propertyTypes.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PropertyType, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var agents = new Mock<IRepository<Agent>>();
        agents.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Agent, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Properties).Returns(propertyRepo.Object);
        unitOfWork.Setup(u => u.PropertyTypes).Returns(propertyTypes.Object);
        unitOfWork.Setup(u => u.Agents).Returns(agents.Object);
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var agentService = new Mock<IAgentService>();
        agentService.Setup(s => s.GetOrCreateAgentIdForCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(agentId);

        var phoneVerification = new Mock<IUserPhoneVerificationService>();
        phoneVerification.Setup(s => s.EnsureCanPostListingAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(u => u.IsInRole(AppRoles.Broker)).Returns(true);
        currentUser.Setup(u => u.IsInRole(AppRoles.Customer)).Returns(false);

        var handler = new CreatePropertyCommandHandler(
            unitOfWork.Object, agentService.Object, phoneVerification.Object, currentUser.Object);
        var command = new CreatePropertyCommand
        {
            Title = "Căn hộ mới",
            Description = "Test",
            Price = 2_000_000_000m,
            Area = 65m,
            ListingType = ListingType.ForSale,
            Street = "1 Test",
            Ward = "Phường 1",
            District = "Quận 1",
            Province = "TP.HCM",
            PropertyTypeId = propertyTypeId,
            AgentId = agentId
        };

        var id = await handler.Handle(command, CancellationToken.None);

        id.Should().Be(created!.Id);
        created.Status.Should().Be(PropertyStatus.Draft);
        created.Title.Should().Be("Căn hộ mới");
    }

    private static Mock<IUnitOfWork> CreateUnitOfWork(Property property)
    {
        var propertyRepo = new Mock<IPropertyRepository>();
        propertyRepo.Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);
        propertyRepo.Setup(r => r.Update(property));

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Properties).Returns(propertyRepo.Object);
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        return unitOfWork;
    }
}
