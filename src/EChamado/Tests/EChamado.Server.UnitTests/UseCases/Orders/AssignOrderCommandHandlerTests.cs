using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Application.UseCases.Orders.Commands.Handlers;
using EChamado.Server.Application.Users;
using EChamado.Server.Application.Users.Abstractions;
using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Paramore.Brighter;
using Xunit;

namespace EChamado.Server.UnitTests.UseCases.Orders;

public class AssignOrderCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserReadRepository> _userReadRepositoryMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<ILogger<AssignOrderCommandHandler>> _loggerMock;
    private readonly AssignOrderCommandHandler _handler;

    private static readonly Guid OrderId = Guid.NewGuid();
    private static readonly Guid ResponsibleUserId = Guid.NewGuid();

    public AssignOrderCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userReadRepositoryMock = new Mock<IUserReadRepository>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock.SetupGet(x => x.UtcNow).Returns(DateTime.UtcNow);
        _loggerMock = new Mock<ILogger<AssignOrderCommandHandler>>();

        _handler = new AssignOrderCommandHandler(
            _unitOfWorkMock.Object,
            _userReadRepositoryMock.Object,
            _dateTimeProviderMock.Object,
            _loggerMock.Object);
    }

    private Order BuildValidOrder()
    {
        return Order.Create(
            title: "Chamado teste",
            description: "Descrição",
            requestingUserEmail: "req@echamado.com",
            responsibleUserEmail: "old@echamado.com",
            requestingUserId: Guid.NewGuid(),
            responsibleUserId: Guid.NewGuid(),
            categoryId: Guid.NewGuid(),
            departmentId: Guid.NewGuid(),
            orderTypeId: Guid.NewGuid(),
            statusTypeId: Guid.NewGuid(),
            subCategoryId: null,
            dueDate: null,
            dateTimeProvider: _dateTimeProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldAssignOrderWithResolvedEmail()
    {
        // Arrange
        var order = BuildValidOrder();
        var command = new AssignOrderCommand(OrderId, ResponsibleUserId);
        var responsible = new UserDetailsDto(ResponsibleUserId, "responsible@echamado.com", "Responsible", DateTime.UtcNow);

        _unitOfWorkMock.Setup(x => x.Orders.GetByIdAsync(OrderId)).ReturnsAsync(order);
        _userReadRepositoryMock.Setup(x => x.GetByIdAsync(ResponsibleUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responsible);
        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Orders.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Result!.Success.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.Orders.UpdateAsync(It.IsAny<Order>()), Times.Once);
        _userReadRepositoryMock.Verify(x => x.GetByIdAsync(ResponsibleUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new AssignOrderCommand(OrderId, ResponsibleUserId);
        _unitOfWorkMock.Setup(x => x.Orders.GetByIdAsync(OrderId)).ReturnsAsync((Order?)null);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _userReadRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Orders.UpdateAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ResponsibleUserNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var order = BuildValidOrder();
        var command = new AssignOrderCommand(OrderId, ResponsibleUserId);

        _unitOfWorkMock.Setup(x => x.Orders.GetByIdAsync(OrderId)).ReturnsAsync(order);
        _userReadRepositoryMock.Setup(x => x.GetByIdAsync(ResponsibleUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDetailsDto?)null);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _unitOfWorkMock.Verify(x => x.Orders.UpdateAsync(It.IsAny<Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Never);
    }
}
