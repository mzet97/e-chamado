using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Shared.Responses;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Paramore.Brighter;
using Xunit;

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
        _handler = new CreateCategoryCommandHandler(_unitOfWorkMock.Object, _commandProcessorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCategory()
    {
        // Arrange
        var command = new CreateCategoryCommand("Categoria Teste", "Descrição teste");

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
    [InlineData("", "Descrição")]
    [InlineData("   ", "Descrição")]
    [InlineData("\t", "Descrição")]
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
        var command = new CreateCategoryCommand("Categoria", "Descrição");

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
    [InlineData("Categoria Normal", "Descrição normal")]
    [InlineData("CATEGORIA MAIÚSCULA", "DESCRIÇÃO MAIÚSCULA")]
    [InlineData("categoria minúscula", "descrição minúscula")]
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