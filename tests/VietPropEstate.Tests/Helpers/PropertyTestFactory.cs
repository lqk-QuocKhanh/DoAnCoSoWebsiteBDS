using System.Reflection;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.ValueObjects;

namespace VietPropEstate.Tests.Helpers;

internal static class PropertyTestFactory
{
    private static readonly Guid DefaultPropertyTypeId = new("11111111-1111-1111-1111-111111111111");

    public static Property CreateProperty(
        string title = "Nhà test",
        Guid? agentId = null,
        PropertyStatus? targetStatus = null)
    {
        var property = Property.Create(
            title,
            "Mô tả test",
            new Money(3_000_000_000m, "VND"),
            80m,
            ListingType.ForSale,
            new Address("123 Test", "Phường 1", "Quận 1", "TP.HCM"),
            DefaultPropertyTypeId,
            agentId ?? Guid.NewGuid(),
            2,
            2,
            2,
            PropertyDirection.South,
            provinceCode: 79,
            provinceName: "Thành phố Hồ Chí Minh",
            wardCode: 26824,
            wardName: "Phường Thủ Đức");

        ApplyStatus(property, targetStatus ?? PropertyStatus.Draft);
        return property;
    }

    public static Agent CreateAgent(string userId = "user-123")
        => Agent.Create(
            "Agent Test",
            "agent@test.vietpropestate.vn",
            "0901234567",
            licenseNumber: "BRK-TEST",
            agencyName: "VietProp Test",
            userId: userId);

    public static void AttachAgent(Property property, Agent agent)
    {
        typeof(Property).GetProperty(nameof(Property.Agent))!
            .SetValue(property, agent);
    }

    public static void ApplyStatus(Property property, PropertyStatus status)
    {
        switch (status)
        {
            case PropertyStatus.Draft:
                return;
            case PropertyStatus.PendingApproval:
                property.SubmitForApproval();
                return;
            case PropertyStatus.Active:
                property.SubmitForApproval();
                property.Publish();
                return;
            case PropertyStatus.Rejected:
                property.SubmitForApproval();
                property.Reject();
                return;
            case PropertyStatus.Withdrawn:
                property.SubmitForApproval();
                property.Publish();
                property.Withdraw();
                return;
            case PropertyStatus.UnderOffer:
                property.SubmitForApproval();
                property.Publish();
                property.PlaceUnderOffer();
                return;
            case PropertyStatus.Sold:
                property.SubmitForApproval();
                property.Publish();
                property.MarkAsSold();
                return;
            case PropertyStatus.Rented:
                property.SubmitForApproval();
                property.Publish();
                property.MarkAsRented();
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }
}
