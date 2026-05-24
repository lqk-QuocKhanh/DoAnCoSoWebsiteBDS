using FluentAssertions;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Exceptions;
using VietPropEstate.Tests.Helpers;

namespace VietPropEstate.Tests.Domain;

public class PropertyWorkflowTests
{
    [Fact]
    public void SubmitForApproval_FromDraft_SetsPendingApproval()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.Draft);

        property.SubmitForApproval();

        property.Status.Should().Be(PropertyStatus.PendingApproval);
    }

    [Fact]
    public void SubmitForApproval_FromActive_Throws()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.Active);

        var act = () => property.SubmitForApproval();

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be submitted*");
    }

    [Fact]
    public void Publish_FromPendingApproval_SetsActiveAndPublishedAt()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.PendingApproval);

        property.Publish();

        property.Status.Should().Be(PropertyStatus.Active);
        property.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public void Publish_FromDraft_Throws()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.Draft);

        var act = () => property.Publish();

        act.Should().Throw<DomainException>()
            .WithMessage("*pending approval*");
    }

    [Fact]
    public void Reject_FromPendingApproval_SetsRejected()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.PendingApproval);

        property.Reject();

        property.Status.Should().Be(PropertyStatus.Rejected);
    }

    [Fact]
    public void FullListingWorkflow_DraftToActive_Works()
    {
        var property = PropertyTestFactory.CreateProperty(targetStatus: PropertyStatus.Draft);

        property.SubmitForApproval();
        property.Publish();

        property.Status.Should().Be(PropertyStatus.Active);
    }

    [Fact]
    public void AddImage_FirstImage_BecomesPrimary()
    {
        var property = PropertyTestFactory.CreateProperty();
        var image = PropertyImage.Create(property.Id, "/uploads/test.jpg", displayOrder: 0, isPrimary: true);

        property.AddImage(image);

        property.Images.Should().ContainSingle(i => i.IsPrimary);
    }
}
