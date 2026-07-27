using EChamado.Server.Application.Users;
using EChamado.Shared.Services;
using EChamado.Server.Application.Users.Abstractions;
using EChamado.Server.Application.Users.Handlers;
using EChamado.Server.Application.Users.Queries;
using Moq;
using Xunit;

namespace EChamado.Tests.Server.UnitTests.Application.Users;

public sealed class GetUserByEmailHandlerTests
{
    private readonly Mock<IUserReadRepository> _repositoryMock = new();
    private readonly GetUserByEmailHandler _handler;

    public GetUserByEmailHandlerTests()
    {
        _handler = new GetUserByEmailHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarUsuario_QuandoEncontrado()
    {
        var expected = new UserDetailsDto(
            Guid.NewGuid(),
            "user@test.com",
            "User Test",
            DateTime.UtcNow);

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(expected.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var query = new GetUserByEmailQuery(expected.Email);
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        Assert.Equal(expected, result.Result);
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarNull_QuandoNaoEncontrado()
    {
        _repositoryMock
            .Setup(r => r.GetByEmailAsync("missing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDetailsDto?)null);

        var query = new GetUserByEmailQuery("missing@test.com");
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        Assert.Null(result.Result);
    }
}
