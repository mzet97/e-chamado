using EChamado.Server.Domain.Domains.Identities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Echamado.Auth.UnitTests.Models;

public class ApplicationUserTests
{
    [Fact]
    public void ApplicationUser_ShouldInheritFromIdentityUser()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.Should().BeAssignableTo<IdentityUser<Guid>>();
    }

    [Fact]
    public void ApplicationUser_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBe(Guid.Empty);
        user.UserName.Should().BeNull(); // Inicialmente null até ser definido
        user.Email.Should().BeNull(); // Inicialmente null até ser definido
    }

    [Fact]
    public void ApplicationUser_WhenSetUserName_ShouldRetainValue()
    {
        // Arrange
        var user = new ApplicationUser();
        var userName = "testuser";

        // Act
        user.UserName = userName;

        // Assert
        user.UserName.Should().Be(userName);
    }

    [Fact]
    public void ApplicationUser_WhenSetEmail_ShouldRetainValue()
    {
        // Arrange
        var user = new ApplicationUser();
        var email = "test@example.com";

        // Act
        user.Email = email;

        // Assert
        user.Email.Should().Be(email);
    }

    [Theory]
    [InlineData("user1", "user1@example.com")]
    [InlineData("admin", "admin@company.com")]
    [InlineData("test.user", "test.user@domain.org")]
    public void ApplicationUser_WithValidData_ShouldSetPropertiesCorrectly(string userName, string email)
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.UserName = userName;
        user.Email = email;

        // Assert
        user.UserName.Should().Be(userName);
        user.Email.Should().Be(email);
    }

    [Fact]
    public void ApplicationUser_ShouldHaveUniqueId()
    {
        // Arrange & Act
        var user1 = new ApplicationUser();
        var user2 = new ApplicationUser();

        // Assert
        user1.Id.Should().NotBe(user2.Id);
        user1.Id.Should().NotBe(Guid.Empty);
        user2.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void ApplicationUser_ShouldSupportEmailConfirmation()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.EmailConfirmed = true;

        // Assert
        user.EmailConfirmed.Should().BeTrue();
    }

    [Fact]
    public void ApplicationUser_ShouldSupportPhoneNumber()
    {
        // Arrange
        var user = new ApplicationUser();
        var phoneNumber = "+1234567890";

        // Act
        user.PhoneNumber = phoneNumber;

        // Assert
        user.PhoneNumber.Should().Be(phoneNumber);
    }

    [Fact]
    public void ApplicationUser_ShouldSupportTwoFactorAuthentication()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.TwoFactorEnabled = true;

        // Assert
        user.TwoFactorEnabled.Should().BeTrue();
    }

    [Fact]
    public void ApplicationUser_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.EmailConfirmed.Should().BeFalse();
        user.PhoneNumberConfirmed.Should().BeFalse();
        user.TwoFactorEnabled.Should().BeFalse();
        user.LockoutEnabled.Should().BeFalse();
        user.AccessFailedCount.Should().Be(0);
    }

    [Fact]
    public void ApplicationUser_SecurityStamp_ShouldBeGeneratedAutomatically()
    {
        // Arrange & Act
        var user1 = new ApplicationUser();
        var user2 = new ApplicationUser();

        // Assert - SecurityStamp pode ser null ou string vazia por padrão
        user1.SecurityStamp.Should().NotBeNull();
        user2.SecurityStamp.Should().NotBeNull();
    }

    [Fact]
    public void ApplicationUser_ConcurrencyStamp_ShouldBeGenerated()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.ConcurrencyStamp.Should().NotBeNull();
    }

    [Fact]
    public void ApplicationUser_ShouldSupportLockout()
    {
        // Arrange
        var user = new ApplicationUser();
        var lockoutEnd = DateTimeOffset.UtcNow.AddHours(1);

        // Act
        user.LockoutEnabled = true;
        user.LockoutEnd = lockoutEnd;

        // Assert
        user.LockoutEnabled.Should().BeTrue();
        user.LockoutEnd.Should().Be(lockoutEnd);
    }

    [Fact]
    public void ApplicationUser_ShouldHavePhotoProperty()
    {
        // Arrange
        var user = new ApplicationUser();
        var photoUrl = "https://example.com/photo.jpg";

        // Act
        user.Photo = photoUrl;

        // Assert
        user.Photo.Should().Be(photoUrl);
    }

    [Fact]
    public void ApplicationUser_ShouldHaveNavigationProperties()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.Claims.Should().NotBeNull();
        user.UserRoles.Should().NotBeNull();
        user.Logins.Should().NotBeNull();
        user.Tokens.Should().NotBeNull();
    }
}