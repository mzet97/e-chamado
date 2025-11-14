using AutoFixture;
using EChamado.Server.Domain.Domains.Orders;

namespace EChamado.Server.UnitTests.Common.Builders;

/// <summary>
/// Builder para criar instâncias de Order para testes
/// </summary>
public class OrderTestBuilder
{
    private readonly Fixture _fixture;
    private string _title;
    private string _description;
    private string _requestingUserEmail;
    private string _responsibleUserEmail;
    private Guid _requestingUserId;
    private Guid _responsibleUserId;
    private Guid _categoryId;
    private Guid _departmentId;
    private Guid _orderTypeId;
    private Guid _statusTypeId;
    private Guid? _subCategoryId;
    private DateTime? _dueDate;

    public OrderTestBuilder()
    {
        _fixture = new Fixture();
        // Valores padrão
        _title = _fixture.Create<string>();
        _description = _fixture.Create<string>();
        _requestingUserEmail = _fixture.Create<string>() + "@test.com";
        _responsibleUserEmail = _fixture.Create<string>() + "@test.com";
        _requestingUserId = Guid.NewGuid();
        _responsibleUserId = Guid.NewGuid();
        _categoryId = Guid.NewGuid();
        _departmentId = Guid.NewGuid();
        _orderTypeId = Guid.NewGuid();
        _statusTypeId = Guid.NewGuid();
        _subCategoryId = null;
        _dueDate = DateTime.UtcNow.AddDays(30);
    }

    public static OrderTestBuilder Create() => new();

    public OrderTestBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public OrderTestBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public OrderTestBuilder WithRequestingUserEmail(string email)
    {
        _requestingUserEmail = email;
        return this;
    }

    public OrderTestBuilder WithResponsibleUserEmail(string email)
    {
        _responsibleUserEmail = email;
        return this;
    }

    public OrderTestBuilder WithRequestingUserId(Guid userId)
    {
        _requestingUserId = userId;
        return this;
    }

    public OrderTestBuilder WithResponsibleUserId(Guid userId)
    {
        _responsibleUserId = userId;
        return this;
    }

    public OrderTestBuilder WithCategoryId(Guid categoryId)
    {
        _categoryId = categoryId;
        return this;
    }

    public OrderTestBuilder WithDepartmentId(Guid departmentId)
    {
        _departmentId = departmentId;
        return this;
    }

    public OrderTestBuilder WithOrderTypeId(Guid orderTypeId)
    {
        _orderTypeId = orderTypeId;
        return this;
    }

    public OrderTestBuilder WithStatusTypeId(Guid statusTypeId)
    {
        _statusTypeId = statusTypeId;
        return this;
    }

    public OrderTestBuilder WithSubCategoryId(Guid? subCategoryId)
    {
        _subCategoryId = subCategoryId;
        return this;
    }

    public OrderTestBuilder WithDueDate(DateTime? dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public OrderTestBuilder WithValidData()
    {
        return WithTitle("Test Order")
            .WithDescription("Test order description")
            .WithRequestingUserEmail("requester@test.com")
            .WithResponsibleUserEmail("responsible@test.com")
            .WithRequestingUserId(Guid.NewGuid())
            .WithResponsibleUserId(Guid.NewGuid())
            .WithCategoryId(Guid.NewGuid())
            .WithDepartmentId(Guid.NewGuid())
            .WithOrderTypeId(Guid.NewGuid())
            .WithStatusTypeId(Guid.NewGuid())
            .WithDueDate(DateTime.UtcNow.AddDays(30));
    }

    public OrderTestBuilder WithInvalidTitle()
    {
        return WithTitle(string.Empty);
    }

    public OrderTestBuilder WithInvalidDescription()
    {
        return WithDescription(string.Empty);
    }

    public OrderTestBuilder WithExpiredDueDate()
    {
        return WithDueDate(DateTime.UtcNow.AddDays(-1));
    }

    public Order Build()
    {
        return Order.Create(
            _title,
            _description,
            _requestingUserEmail,
            _responsibleUserEmail,
            _requestingUserId,
            _responsibleUserId,
            _categoryId,
            _departmentId,
            _orderTypeId,
            _statusTypeId,
            _subCategoryId,
            _dueDate);
    }
}