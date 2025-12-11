using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Shared.Services;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Paramore.Brighter;
using Xunit;
using EChamado.Server.Application.UseCases.Categories.Commands.Handlers;

namespace EChamado.Server.UnitTests.UseCases.Categories;

public class CreateCategoryCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAmACommandProcessor> _commandProcessorMock;
    private readonly Mock<ILogger<CreateCategoryCommandHandler>> _loggerMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _commandProcessorMock = new Mock<IAmACommandProcessor>();
        _loggerMock = new Mock<ILogger<CreateCategoryCommandHandler>>();
        _handler = new CreateCategoryCommandHandler(
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
    public async Task Handle_ValidCommand_ShouldCreateCategory()
    {
        // Arrange
        var command = new CreateCategoryCommand("Categoria Teste", "Descri��o teste");

        _unitOfWorkMock
            .Setup(x => x.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.Categories.AddAsync(It.IsAny<Category>()))
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
        _unitOfWorkMock.Verify(x => x.Categories.AddAsync(It.IsAny<Category>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", "Descri��o")]
    [InlineData("   ", "Descri��o")]
    [InlineData("\t", "Descri��o")]
    public async Task Handle_InvalidName_ShouldThrowValidationException(string name, string description)
    {
        // Arrange
        var command = new CreateCategoryCommand(name, description);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Never);
        _unitOfWorkMock.Verify(x => x.Categories.AddAsync(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_DatabaseError_ShouldThrowException()
    {
        // Arrange
        var command = new CreateCategoryCommand("Categoria", "Descri��o");

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Categories.AddAsync(It.IsAny<Category>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
        exception.Message.Should().Be("Database error");

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.Categories.AddAsync(It.IsAny<Category>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Theory]
    [InlineData("Categoria Normal", "Descri��o normal")]
    [InlineData("CATEGORIA MAI�SCULA", "DESCRI��O MAI�SCULA")]
    [InlineData("categoria min�scula", "descri��o min�scula")]
    public async Task Handle_DifferentValidInputs_ShouldCreateCategory(string name, string description)
    {
        // Arrange
        var command = new CreateCategoryCommand(name, description);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Categories.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Result!.Success.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.Categories.AddAsync(It.IsAny<Category>()), Times.Once);
    }
}