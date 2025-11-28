using EChamado.Server.Application.UseCases.Comments.Commands;
using EChamado.Shared.Services;
using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Server.UnitTests.Common.Builders;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Paramore.Brighter;
using Xunit;

namespace EChamado.Server.UnitTests.UseCases.Comments;

public class CreateCommentCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAmACommandProcessor> _commandProcessorMock;
    private readonly Mock<ILogger<CreateCommentCommandHandler>> _loggerMock;
    private readonly CreateCommentCommandHandler _handler;

    public CreateCommentCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _commandProcessorMock = new Mock<IAmACommandProcessor>();
        _loggerMock = new Mock<ILogger<CreateCommentCommandHandler>>();
        _handler = new CreateCommentCommandHandler(
            _unitOfWorkMock.Object, 
            _commandProcessorMock.Object,
            new MockDateTimeProvider(),
            _loggerMock.Object);
    }

    private class MockDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTimeOffset OffsetNow => DateTimeOffset.Now;
        public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateComment()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateCommentCommand(
            "Coment�rio teste",
            orderId,
            userId,
            "user@example.com"
        );

        var order = OrderTestBuilder.Create()
            .WithValidData()
            .Build();

        _unitOfWorkMock
            .Setup(x => x.Orders.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Comments.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull();
        result.Result!.Success.Should().BeTrue();
        ((BaseResult<Guid>)result.Result).Data.Should().NotBeEmpty();

        _unitOfWorkMock.Verify(x => x.Orders.GetByIdAsync(orderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.Comments.AddAsync(It.IsAny<Comment>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new CreateCommentCommand(
            "Coment�rio",
            orderId,
            Guid.NewGuid(),
            "user@example.com"
        );

        _unitOfWorkMock
            .Setup(x => x.Orders.GetByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Order {orderId} not found");

        _unitOfWorkMock.Verify(x => x.Orders.GetByIdAsync(orderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Never);
        _unitOfWorkMock.Verify(x => x.Comments.AddAsync(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task Handle_InvalidCommentText_ShouldThrowValidationException(string text)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateCommentCommand(text, orderId, userId, "user@example.com");

        var order = OrderTestBuilder.Create()
            .WithValidData()
            .Build();

        _unitOfWorkMock
            .Setup(x => x.Orders.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _unitOfWorkMock.Verify(x => x.Orders.GetByIdAsync(orderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.Comments.AddAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@invalid.com")]
    [InlineData("invalid@")]
    public async Task Handle_InvalidEmail_ShouldThrowValidationException(string email)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateCommentCommand("Coment�rio v�lido", orderId, userId, email);

        var order = OrderTestBuilder.Create()
            .WithValidData()
            .Build();

        _unitOfWorkMock
            .Setup(x => x.Orders.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _unitOfWorkMock.Verify(x => x.Orders.GetByIdAsync(orderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.Comments.AddAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DatabaseError_ShouldThrowException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new CreateCommentCommand("Coment�rio", orderId, Guid.NewGuid(), "user@example.com");

        var order = OrderTestBuilder.Create().WithValidData().Build();

        _unitOfWorkMock.Setup(x => x.Orders.GetByIdAsync(orderId)).ReturnsAsync(order);
        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Comments.AddAsync(It.IsAny<Comment>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
        exception.Message.Should().Be("Database error");

        _unitOfWorkMock.Verify(x => x.Orders.GetByIdAsync(orderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.Comments.AddAsync(It.IsAny<Comment>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_CommitError_ShouldThrowException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new CreateCommentCommand("Coment�rio", orderId, Guid.NewGuid(), "user@example.com");

        var order = OrderTestBuilder.Create().WithValidData().Build();

        _unitOfWorkMock.Setup(x => x.Orders.GetByIdAsync(orderId)).ReturnsAsync(order);
        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Comments.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync())
            .ThrowsAsync(new InvalidOperationException("Commit failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
        exception.Message.Should().Be("Commit failed");

        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }
}