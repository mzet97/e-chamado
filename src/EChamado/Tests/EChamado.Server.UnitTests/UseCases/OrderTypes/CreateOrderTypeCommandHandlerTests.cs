using EChamado.Server.Application.UseCases.OrderTypes.Commands;
using EChamado.Shared.Services;
using EChamado.Server.Domain.Domains.Orders.Entities;
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
using EChamado.Server.Application.UseCases.OrderTypes.Commands.Handlers;

namespace EChamado.Server.UnitTests.UseCases.OrderTypes;

public class CreateOrderTypeCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAmACommandProcessor> _commandProcessorMock;
    private readonly Mock<ILogger<CreateOrderTypeCommandHandler>> _loggerMock;
    private readonly CreateOrderTypeCommandHandler _handler;

    public CreateOrderTypeCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _commandProcessorMock = new Mock<IAmACommandProcessor>();
        _loggerMock = new Mock<ILogger<CreateOrderTypeCommandHandler>>();
        _handler = new CreateOrderTypeCommandHandler(
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
    public async Task Handle_ValidCommand_ShouldCreateOrderType()
    {
        // Arrange
        var command = new CreateOrderTypeCommand("Suporte T�cnico", "Solicita��es de suporte t�cnico");

        _unitOfWorkMock
            .Setup(x => x.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull();
        result.Result!.Success.Should().BeTrue();
        ((BaseResult<Guid>)result.Result).Data.Should().NotBeEmpty();

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", "Descri��o v�lida")]
    [InlineData("   ", "Descri��o v�lida")]
    [InlineData("\t", "Descri��o v�lida")]
    public async Task Handle_InvalidName_ShouldThrowValidationException(string invalidName, string description)
    {
        // Arrange
        var command = new CreateOrderTypeCommand(invalidName, description);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Never);
        _unitOfWorkMock.Verify(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Theory]
    [InlineData("Nome v�lido", "")]
    [InlineData("Nome v�lido", "   ")]
    [InlineData("Nome v�lido", "\t")]
    public async Task Handle_InvalidDescription_ShouldThrowValidationException(string name, string invalidDescription)
    {
        // Arrange
        var command = new CreateOrderTypeCommand(name, invalidDescription);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Never);
        _unitOfWorkMock.Verify(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_DatabaseError_ShouldThrowException()
    {
        // Arrange
        var command = new CreateOrderTypeCommand("Tipo V�lido", "Descri��o v�lida");

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
        exception.Message.Should().Be("Database error");

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_CommitError_ShouldThrowException()
    {
        // Arrange
        var command = new CreateOrderTypeCommand("Tipo V�lido", "Descri��o v�lida");

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync())
            .ThrowsAsync(new InvalidOperationException("Commit failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
        exception.Message.Should().Be("Commit failed");

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Theory]
    [InlineData("Suporte T�cnico", "Solicita��es t�cnicas")]
    [InlineData("Recursos Humanos", "Quest�es de RH")]
    [InlineData("Financeiro", "Quest�es financeiras")]
    [InlineData("Infraestrutura", "Quest�es de infraestrutura")]
    public async Task Handle_DifferentValidInputs_ShouldCreateOrderType(string name, string description)
    {
        // Arrange
        var command = new CreateOrderTypeCommand(name, description);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Result!.Success.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetCorrectResultData()
    {
        // Arrange
        var command = new CreateOrderTypeCommand("Test OrderType", "Test Description");
        var capturedOrderType = (OrderType?)null;

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()))
            .Callback<OrderType>(c => capturedOrderType = c)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        capturedOrderType.Should().NotBeNull();
        capturedOrderType!.Name.Should().Be("Test OrderType");
        capturedOrderType.Description.Should().Be("Test Description");

        var resultData = ((BaseResult<Guid>)result.Result!).Data;
        resultData.Should().Be(capturedOrderType.Id);
    }

    [Fact]
    public async Task Handle_WithNullCommand_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(null!));
    }

    [Fact]
    public async Task Handle_MaxLengthInput_ShouldCreateOrderType()
    {
        // Arrange
        var maxName = new string('A', 100); // Assumindo limite de 100
        var maxDescription = new string('B', 500); // Assumindo limite de 500
        var command = new CreateOrderTypeCommand(maxName, maxDescription);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Result!.Success.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TooLongName_ShouldThrowValidationException()
    {
        // Arrange
        var tooLongName = new string('A', 101); // Excede limite
        var command = new CreateOrderTypeCommand(tooLongName, "Descri��o");

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.HandleAsync(command));
        _unitOfWorkMock.Verify(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldGenerateUniqueIds()
    {
        // Arrange
        var command1 = new CreateOrderTypeCommand("Tipo 1", "Descri��o 1");
        var command2 = new CreateOrderTypeCommand("Tipo 2", "Descri��o 2");
        var orderTypeIds = new List<Guid>();

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>()))
            .Callback<OrderType>(c => orderTypeIds.Add(c.Id))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command1);
        await _handler.HandleAsync(command2);

        // Assert
        orderTypeIds.Should().HaveCount(2);
        orderTypeIds[0].Should().NotBe(orderTypeIds[1]);
        orderTypeIds.Should().OnlyContain(id => id != Guid.Empty);
    }

    [Fact]
    public async Task Handle_ConcurrentRequests_ShouldHandleCorrectly()
    {
        // Arrange
        var command1 = new CreateOrderTypeCommand("Concurrent 1", "Description 1");
        var command2 = new CreateOrderTypeCommand("Concurrent 2", "Description 2");

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.OrderTypes.AddAsync(It.IsAny<OrderType>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var task1 = _handler.HandleAsync(command1);
        var task2 = _handler.HandleAsync(command2);
        var results = await Task.WhenAll(task1, task2);

        // Assert
        results.Should().HaveCount(2);
        results.Should().OnlyContain(r => r.Result!.Success);
    }
}