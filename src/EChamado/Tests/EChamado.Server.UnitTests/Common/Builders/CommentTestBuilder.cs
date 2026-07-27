using AutoFixture;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Services;

namespace EChamado.Server.UnitTests.Common.Builders;

public class CommentTestBuilder
{
    private readonly Fixture _fixture;
    private string _text;
    private Guid _orderId;
    private Guid _userId;
    private string _userEmail;
    private bool _textSet;
    private bool _orderIdSet;
    private bool _userIdSet;
    private bool _userEmailSet;
    private static readonly IDateTimeProvider _dateTimeProvider = new MockDateTimeProvider();

    public CommentTestBuilder()
    {
        _fixture = new Fixture();
        _text = _fixture.Create<string>();
        _orderId = Guid.NewGuid();
        _userId = Guid.NewGuid();
        _userEmail = _fixture.Create<string>() + "@test.com";
    }

    private class MockDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTimeOffset OffsetNow => DateTimeOffset.Now;
        public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
    }

    public static CommentTestBuilder Create() => new();

    public CommentTestBuilder WithText(string text)
    {
        _text = text;
        _textSet = true;
        return this;
    }

    public CommentTestBuilder WithOrderId(Guid orderId)
    {
        _orderId = orderId;
        _orderIdSet = true;
        return this;
    }

    public CommentTestBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        _userIdSet = true;
        return this;
    }

    public CommentTestBuilder WithUserEmail(string userEmail)
    {
        _userEmail = userEmail;
        _userEmailSet = true;
        return this;
    }

    public CommentTestBuilder WithValidData()
    {
        if (!_textSet)
        {
            WithText("Test comment content");
        }

        if (!_orderIdSet)
        {
            WithOrderId(Guid.NewGuid());
        }

        if (!_userIdSet)
        {
            WithUserId(Guid.NewGuid());
        }

        if (!_userEmailSet)
        {
            WithUserEmail("user@test.com");
        }

        return this;
    }

    public CommentTestBuilder WithEmptyText() => WithText(string.Empty);

    public CommentTestBuilder WithLongText() => WithText(new string('A', 5001));

    public CommentTestBuilder WithInvalidEmail() => WithUserEmail("invalid-email");

    public Comment Build()
    {
        return Comment.Create(_text, _orderId, _userId, _userEmail, _dateTimeProvider);
    }
}
