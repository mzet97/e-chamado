using EChamado.Server.Application.UseCases.Comments.Commands;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EChamado.Server.UnitTests.UseCases.Comments;

public class CreateCommentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<CreateCommentCommandHandler>> _loggerMock;
    private readonly CreateCommentCommandHandler _handler;

    public CreateCommentCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<CreateCommentCommandHandler>>();
        _handler = new CreateCommentCommandHandler(_unitOfWorkMock.Object, _mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateComment()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateCommentCommand(
            "Comentário teste",
            orderId,
            userId,
            "user@example.com"
        );

        var order = Order.Create(
            "Título",
            "Descrição",
            "user@example.com",
            "resp@example.com",
            userId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            DateTime.Now.AddDays(7)
        );

        _unitOfWorkMock
            .Setup(x => x.Orders.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Comments.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        _unitOfWorkMock.Verify(x => x.Comments.AddAsync(It.IsAny<Comment>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new CreateCommentCommand(
            "Comentário",
            orderId,
            Guid.NewGuid(),
            "user@example.com"
        );

        _unitOfWorkMock
            .Setup(x => x.Orders.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Order {orderId} not found");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Handle_InvalidComment_ShouldThrowValidationException(string text)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateCommentCommand(text, orderId, userId, "user@example.com");

        var order = Order.Create(
            "Título", "Descrição", "user@example.com", "resp@example.com",
            userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), null, DateTime.Now.AddDays(7)
        );

        _unitOfWorkMock
            .Setup(x => x.Orders.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldPublishNotification()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateCommentCommand("Comentário", orderId, userId, "user@example.com");

        var order = Order.Create(
            "Título", "Descrição", "user@example.com", "resp@example.com",
            userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), null, DateTime.Now.AddDays(7)
        );

        _unitOfWorkMock
            .Setup(x => x.Orders.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Comments.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(
            x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
