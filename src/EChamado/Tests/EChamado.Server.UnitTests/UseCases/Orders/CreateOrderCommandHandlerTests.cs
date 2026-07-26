using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Application.UseCases.Orders.Commands.Handlers;
using DomainOrders = EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Paramore.Brighter;
using System.Linq.Expressions;
using Xunit;

namespace EChamado.Server.UnitTests.UseCases.Orders;

public class CreateOrderCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IDateTimeProvider> _dt;
    private readonly Mock<ILogger<CreateOrderCommandHandler>> _log;
    private readonly CreateOrderCommandHandler _handler;
    private readonly StatusType _defaultStatus;

    private static readonly Guid CatId = Guid.NewGuid();
    private static readonly Guid DeptId = Guid.NewGuid();
    private static readonly Guid TypeId = Guid.NewGuid();

    public CreateOrderCommandHandlerTests()
    {
        _uow = new Mock<IUnitOfWork>();
        _dt = new Mock<IDateTimeProvider>();
        _dt.SetupGet(x => x.UtcNow).Returns(DateTime.UtcNow);
        _log = new Mock<ILogger<CreateOrderCommandHandler>>();
        _handler = new CreateOrderCommandHandler(_uow.Object, _dt.Object, _log.Object);
        _defaultStatus = StatusType.Create("Aberto", "Aberto", _dt.Object);
    }

    private void SetupStatusMock(List<StatusType> items)
    {
        // Mock SearchAsync (4 params overload usada pelo handler)
        _uow.Setup(x => x.StatusTypes.SearchAsync(
                It.IsAny<Expression<Func<StatusType, bool>>>(),
                null,
                10, 1))
            .ReturnsAsync(new BaseResultList<StatusType>(items, new PagedResult()));
    }

    private void SetupValidMocks()
    {
        SetupStatusMock(new List<StatusType> { _defaultStatus });
        _uow.Setup(x => x.Categories.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(true);
        _uow.Setup(x => x.Departments.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>()))
            .ReturnsAsync(true);
        _uow.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _uow.Setup(x => x.Orders.AddAsync(It.IsAny<DomainOrders.Order>())).Returns(Task.CompletedTask);
        _uow.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
    }

    private CreateOrderCommand ValidCommand() => new()
    {
        Title = "Chamado teste",
        Description = "Descrição do chamado",
        TypeId = TypeId,
        CategoryId = CatId,
        DepartmentId = DeptId,
        RequestingUserId = Guid.NewGuid(),
        RequestingUserEmail = "user@test.com"
    };

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateOrderAndReturnSuccess()
    {
        // Arrange
        SetupValidMocks();

        // Act
        var result = await _handler.HandleAsync(ValidCommand());

        // Assert
        result.Result!.Success.Should().BeTrue();
        ((BaseResult<Guid>)result.Result).Data.Should().NotBeEmpty();
        _uow.Verify(x => x.Orders.AddAsync(It.IsAny<DomainOrders.Order>()), Times.Once);
        _uow.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_CategoryNull_ShouldThrowValidationException()
    {
        SetupValidMocks();
        var cmd = ValidCommand();
        cmd.CategoryId = null;

        await _handler.Invoking(h => h.HandleAsync(cmd))
            .Should().ThrowAsync<ValidationException>();
        _uow.Verify(x => x.Orders.AddAsync(It.IsAny<DomainOrders.Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DepartmentNull_ShouldThrowValidationException()
    {
        SetupValidMocks();
        var cmd = ValidCommand();
        cmd.DepartmentId = null;

        await _handler.Invoking(h => h.HandleAsync(cmd))
            .Should().ThrowAsync<ValidationException>();
        _uow.Verify(x => x.Orders.AddAsync(It.IsAny<DomainOrders.Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CategoryNotInDb_ShouldThrowNotFoundException()
    {
        SetupValidMocks();
        _uow.Setup(x => x.Categories.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(false);

        await _handler.Invoking(h => h.HandleAsync(ValidCommand()))
            .Should().ThrowAsync<NotFoundException>();
        _uow.Verify(x => x.BeginTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_DepartmentNotInDb_ShouldThrowNotFoundException()
    {
        SetupValidMocks();
        _uow.Setup(x => x.Departments.ExistsAsync(It.IsAny<Expression<Func<Department, bool>>>()))
            .ReturnsAsync(false);

        await _handler.Invoking(h => h.HandleAsync(ValidCommand()))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NoStatusAvailable_ShouldThrowNotFoundException()
    {
        SetupStatusMock(new List<StatusType>()); // vazio

        await _handler.Invoking(h => h.HandleAsync(ValidCommand()))
            .Should().ThrowAsync<NotFoundException>();
    }
}
