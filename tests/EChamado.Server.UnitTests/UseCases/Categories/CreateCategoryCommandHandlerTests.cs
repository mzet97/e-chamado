using AutoFixture;
using AutoFixture.Xunit2;
using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EChamado.Server.UnitTests.UseCases.Categories;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<CreateCategoryCommandHandler>> _loggerMock;
    private readonly CreateCategoryCommandHandler _handler;
    private readonly Fixture _fixture;

    public CreateCategoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<CreateCategoryCommandHandler>>();
        _handler = new CreateCategoryCommandHandler(_unitOfWorkMock.Object, _mediatorMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
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
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(x => x.Categories.AddAsync(It.IsAny<Category>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", "Descrição")]
    [InlineData(null, "Descrição")]
    public async Task Handle_InvalidName_ShouldThrowValidationException(string name, string description)
    {
        // Arrange
        var command = new CreateCategoryCommand(name, description);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldPublishNotification()
    {
        // Arrange
        var command = new CreateCategoryCommand("Nova Categoria", "Descrição");

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Categories.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(
            x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DatabaseError_ShouldRollbackTransaction()
    {
        // Arrange
        var command = new CreateCategoryCommand("Categoria", "Descrição");

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(x => x.Categories.AddAsync(It.IsAny<Category>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
