using FluentAssertions;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Tests.Domain;

public class ConversationAndFavoriteTests
{
    [Fact]
    public void Favorite_Create_RequiresUserAndProperty()
    {
        var favorite = Favorite.Create("user-1", Guid.NewGuid(), "note");

        favorite.UserId.Should().Be("user-1");
        favorite.Note.Should().Be("note");
    }

    [Fact]
    public void Favorite_Create_ThrowsWhenUserMissing()
    {
        var act = () => Favorite.Create("", Guid.NewGuid());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Conversation_Create_SetsParticipants()
    {
        var propertyId = Guid.NewGuid();
        var conversation = Conversation.Create(propertyId, "buyer-1", "seller-1", "Inquiry");

        conversation.PropertyId.Should().Be(propertyId);
        conversation.BuyerId.Should().Be("buyer-1");
        conversation.SellerId.Should().Be("seller-1");
        conversation.IsClosed.Should().BeFalse();
    }

    [Fact]
    public void Conversation_UpdateLastMessage_IncrementsRecipientUnread()
    {
        var conversation = Conversation.Create(Guid.NewGuid(), "buyer-1", "seller-1");
        conversation.UpdateLastMessage("Hello", "buyer-1");

        conversation.SellerUnreadCount.Should().Be(1);
        conversation.GetUnreadCount("seller-1").Should().Be(1);
    }

    [Fact]
    public void Conversation_Create_ThrowsWhenSameParticipant()
    {
        var act = () => Conversation.Create(Guid.NewGuid(), "same-user", "same-user");

        act.Should().Throw<DomainException>();
    }
}
