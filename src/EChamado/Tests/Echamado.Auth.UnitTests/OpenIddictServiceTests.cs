using EChamado.Server.Application.Services;
using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Domain.Services.Interface;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Echamado.Auth.UnitTests;

public class OpenIddictServiceTests
{
    private readonly Mock<IApplicationUserService> _appUserServiceMock;
    private readonly OpenIddictService _service;

    private static readonly Guid UserId = Guid.NewGuid();

    public OpenIddictServiceTests()
    {
        _appUserServiceMock = new Mock<IApplicationUserService>();
        _service = new OpenIddictService(_appUserServiceMock.Object);
    }

    private static ApplicationUser BuildUser(string email, string userName)
    {
        return new ApplicationUser
        {
            Id = UserId,
            Email = email,
            UserName = userName,
            EmailConfirmed = true
        };
    }

    [Fact]
    public async Task LoginOpenIddictAsync_UserNotFound_ShouldReturnNull()
    {
        // Arrange
        _appUserServiceMock.Setup(x => x.FindByEmailAsync("nobody@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _service.LoginOpenIddictAsync("nobody@test.com", "pass");

        // Assert
        result.Should().BeNull();
        _appUserServiceMock.Verify(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task LoginOpenIddictAsync_InvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var user = BuildUser("admin@echamado.com", "admin");
        _appUserServiceMock.Setup(x => x.FindByEmailAsync("admin@echamado.com"))
            .ReturnsAsync(user);
        _appUserServiceMock.Setup(x => x.PasswordSignInAsync("admin", "wrong-pass", false, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _service.LoginOpenIddictAsync("admin@echamado.com", "wrong-pass");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginOpenIddictAsync_ValidCredentials_ShouldUseUserNameNotEmail()
    {
        // Arrange
        var user = BuildUser("admin@echamado.com", "admin");
        _appUserServiceMock.Setup(x => x.FindByEmailAsync("admin@echamado.com"))
            .ReturnsAsync(user);
        // A CRÍTICA: PasswordSignInAsync deve ser chamado com UserName ("admin"),
        // não com o Email ("admin@echamado.com")
        _appUserServiceMock.Setup(x => x.PasswordSignInAsync("admin", "Admin@123456", false, false))
            .ReturnsAsync(SignInResult.Success);
        _appUserServiceMock.Setup(x => x.GetClaimsAsync(user)).ReturnsAsync(new List<ApplicationUserClaim>());
        _appUserServiceMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

        // Act
        var result = await _service.LoginOpenIddictAsync("admin@echamado.com", "Admin@123456");

        // Assert
        result.Should().NotBeNull();
        result!.IsAuthenticated.Should().BeTrue();
        // Verifica que PasswordSignInAsync recebeu o UserName, NÃO o Email
        _appUserServiceMock.Verify(x => x.PasswordSignInAsync("admin", "Admin@123456", false, false), Times.Once);
        // E NÃO chamou com o email
        _appUserServiceMock.Verify(x => x.PasswordSignInAsync("admin@echamado.com", It.IsAny<string>(),
            It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task LoginOpenIddictAsync_Success_ShouldReturnIdentityWithClaims()
    {
        // Arrange
        var user = BuildUser("admin@echamado.com", "admin");
        _appUserServiceMock.Setup(x => x.FindByEmailAsync("admin@echamado.com"))
            .ReturnsAsync(user);
        _appUserServiceMock.Setup(x => x.PasswordSignInAsync("admin", "pass", false, false))
            .ReturnsAsync(SignInResult.Success);
        _appUserServiceMock.Setup(x => x.GetClaimsAsync(user))
            .ReturnsAsync(new List<ApplicationUserClaim>
            {
                new() { ClaimType = "custom_claim", ClaimValue = "value", UserId = UserId }
            });
        _appUserServiceMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Admin" });

        // Act
        var result = await _service.LoginOpenIddictAsync("admin@echamado.com", "pass");

        // Assert
        result.Should().NotBeNull();
        var claims = result!.Claims.ToList();
        claims.Should().Contain(c => c.Type == "sub" && c.Value == UserId.ToString());
        claims.Should().Contain(c => c.Type == "role" && c.Value == "Admin");
        claims.Should().Contain(c => c.Type == "custom_claim" && c.Value == "value");
    }
}
