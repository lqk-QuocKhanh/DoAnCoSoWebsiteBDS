using FluentAssertions;
using Moq;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Features.Auth.Commands.LoginUser;
using VietPropEstate.Application.Features.Auth.DTOs;
using VietPropEstate.Application.Features.Properties.Commands.ToggleFavorite;
using VietPropEstate.Application.Features.Properties.Commands.WithdrawProperty;
using VietPropEstate.Application.Features.Properties.Commands.RestoreProperty;
using VietPropEstate.Application.Features.Properties.Commands.MarkPropertyAsSold;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Interfaces;
using VietPropEstate.Tests.Helpers;

namespace VietPropEstate.Tests.Application;

public class FavoriteAndAuthHandlerTests
{
    [Fact]
    public async Task ToggleFavorite_AddsFavorite_WhenNotExists()
    {
        var propertyId = Guid.NewGuid();
        var userId = "user-1";
        Favorite? added = null;

        var propertyRepo = new Mock<IPropertyRepository>();
        propertyRepo.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Property, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var favorites = new Mock<IRepository<Favorite>>();
        favorites.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Favorite, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Favorite?)null);
        favorites.Setup(r => r.AddAsync(It.IsAny<Favorite>(), It.IsAny<CancellationToken>()))
            .Callback<Favorite, CancellationToken>((f, _) => added = f)
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Properties).Returns(propertyRepo.Object);
        unitOfWork.Setup(u => u.Favorites).Returns(favorites.Object);
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new ToggleFavoriteCommandHandler(unitOfWork.Object);
        var result = await handler.Handle(new ToggleFavoriteCommand
        {
            UserId = userId,
            PropertyId = propertyId,
            Note = "Interested"
        }, CancellationToken.None);

        result.Should().BeTrue();
        added.Should().NotBeNull();
        added!.UserId.Should().Be(userId);
        added.PropertyId.Should().Be(propertyId);
    }

    [Fact]
    public async Task ToggleFavorite_RemovesFavorite_WhenExists()
    {
        var favorite = Favorite.Create("user-1", Guid.NewGuid());

        var propertyRepo = new Mock<IPropertyRepository>();
        propertyRepo.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Property, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var favorites = new Mock<IRepository<Favorite>>();
        favorites.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Favorite, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(favorite);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Properties).Returns(propertyRepo.Object);
        unitOfWork.Setup(u => u.Favorites).Returns(favorites.Object);
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new ToggleFavoriteCommandHandler(unitOfWork.Object);
        var result = await handler.Handle(new ToggleFavoriteCommand
        {
            UserId = favorite.UserId,
            PropertyId = favorite.PropertyId
        }, CancellationToken.None);

        result.Should().BeFalse();
        favorites.Verify(f => f.Remove(favorite), Times.Once);
    }

    [Fact]
    public async Task LoginUserCommand_DelegatesToAuthService()
    {
        var authService = new Mock<IAuthService>();
        authService.Setup(s => s.LoginAsync("user@test.com", "pass", "127.0.0.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthResponseDto { Email = "user@test.com", AccessToken = "token" });

        var handler = new LoginUserCommandHandler(authService.Object);
        var result = await handler.Handle(new LoginUserCommand
        {
            Email = "user@test.com",
            Password = "pass",
            IpAddress = "127.0.0.1"
        }, CancellationToken.None);

        result.AccessToken.Should().Be("token");
    }
}

public class PropertyStatusHandlerTests
{
    [Fact]
    public async Task WithdrawPropertyCommand_SetsWithdrawn()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.Active);
        var unitOfWork = CreateUnitOfWork(property);
        var handler = new WithdrawPropertyCommandHandler(unitOfWork.Object);

        await handler.Handle(new WithdrawPropertyCommand(property.Id), CancellationToken.None);

        property.Status.Should().Be(PropertyStatus.Withdrawn);
    }

    [Fact]
    public async Task RestorePropertyCommand_RestoresWithdrawnListing()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.Withdrawn);
        var unitOfWork = CreateUnitOfWork(property);
        var handler = new RestorePropertyCommandHandler(unitOfWork.Object);

        await handler.Handle(new RestorePropertyCommand(property.Id), CancellationToken.None);

        property.Status.Should().Be(PropertyStatus.Active);
    }

    [Fact]
    public async Task MarkPropertyAsSoldCommand_SetsSold()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.Active);
        var unitOfWork = CreateUnitOfWork(property);
        var handler = new MarkPropertyAsSoldCommandHandler(unitOfWork.Object);

        await handler.Handle(new MarkPropertyAsSoldCommand(property.Id), CancellationToken.None);

        property.Status.Should().Be(PropertyStatus.Sold);
    }

    private static Mock<IUnitOfWork> CreateUnitOfWork(Property property)
    {
        var propertyRepo = new Mock<IPropertyRepository>();
        propertyRepo.Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Properties).Returns(propertyRepo.Object);
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        return unitOfWork;
    }
}
