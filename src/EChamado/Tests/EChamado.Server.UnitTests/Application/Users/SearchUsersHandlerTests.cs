using EChamado.Server.Application.Users;
using EChamado.Server.Application.Users.Abstractions;
using EChamado.Server.Application.Users.Handlers;
using EChamado.Server.Application.Users.Queries;
using Moq;
using Xunit;

namespace EChamado.Tests.Server.UnitTests.Application.Users;

public sealed class SearchUsersHandlerTests
{
    private readonly Mock<IUserReadRepository> _repositoryMock = new();
    private readonly SearchUsersHandler _handler;

    public SearchUsersHandlerTests()
    {
        _handler = new SearchUsersHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_DeveNormalizarFiltro()
    {
        var filter = new UserSearchFilter
        {
            Page = -5,
            PageSize = 500,
            SortBy = "email",
            SortDescending = true
        };

        var expected = new PagedResult<UserListItemDto>(
            new List<UserListItemDto>
            {
                new(Guid.NewGuid(), "alpha@test.com", "Alpha", DateTime.UtcNow)
            },
            1,
            200,
            1);

        _repositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<UserSearchFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var query = new SearchUsersQuery(filter);
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        Assert.Equal(expected, result.Result);
        _repositoryMock.Verify(r =>
            r.SearchAsync(It.Is<UserSearchFilter>(f => f.Page == 1 && f.PageSize == 200),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
