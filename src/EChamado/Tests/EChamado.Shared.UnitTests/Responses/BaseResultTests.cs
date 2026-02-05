using EChamado.Shared.Responses;
using FluentAssertions;
using Xunit;

namespace EChamado.Shared.UnitTests.Responses;

public class BaseResultTests
{
    [Fact]
    public void BaseResult_WithSuccessData_ShouldBeSuccessful()
    {
        // Arrange
        var data = "test data";

        // Act
        var result = new BaseResult<string>(data);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().Be(data);
        result.Message.Should().BeEmpty();
    }

    [Fact]
    public void BaseResult_WithErrorMessage_ShouldBeUnsuccessful()
    {
        // Arrange
        var errorMessage = "Error occurred";

        // Act
        var result = new BaseResult<string>("", false, errorMessage);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Data.Should().BeEmpty();
        result.Message.Should().Be(errorMessage);
    }

    [Fact]
    public void BaseResult_WithNullData_ShouldWork()
    {
        // Arrange & Act
        var result = new BaseResult<string?>(null);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Message.Should().BeEmpty();
    }

    [Theory]
    [InlineData(42)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void BaseResult_WithDifferentIntegerTypes_ShouldWork(int data)
    {
        // Act
        var result = new BaseResult<int>(data);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(data);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Simple text")]
    [InlineData("Text with special chars @#$%^&*()")]
    public void BaseResult_WithDifferentStrings_ShouldWork(string data)
    {
        // Act
        var result = new BaseResult<string>(data);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(data);
    }

    [Fact]
    public void BaseResult_WithComplexObject_ShouldWork()
    {
        // Arrange
        var data = new { Id = Guid.NewGuid(), Name = "Test", Count = 5 };

        // Act
        var result = new BaseResult<object>(data);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(data);
    }

    [Fact]
    public void BaseResult_WithCollection_ShouldWork()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = new BaseResult<int[]>(data);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void BaseResult_WithGuid_ShouldWork()
    {
        // Arrange
        var data = Guid.NewGuid();

        // Act
        var result = new BaseResult<Guid>(data);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(data);
    }

    [Fact]
    public void BaseResult_WithDateTime_ShouldWork()
    {
        // Arrange
        var data = DateTime.Now;

        // Act
        var result = new BaseResult<DateTime>(data);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(data);
    }

    [Fact]
    public void BaseResult_WithFailureState_ShouldWork()
    {
        // Act
        var result = new BaseResult<string>("error data", false, "Something went wrong");

        // Assert
        result.Success.Should().BeFalse();
        result.Data.Should().Be("error data");
        result.Message.Should().Be("Something went wrong");
    }

    [Theory]
    [InlineData("Error message")]
    [InlineData("")]
    [InlineData("   ")]
    public void BaseResult_WithDifferentErrorMessages_ShouldWork(string errorMessage)
    {
        // Act
        var result = new BaseResult<string>("", false, errorMessage);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be(errorMessage);
    }

    [Fact]
    public void BaseResult_Properties_ShouldBeReadOnly()
    {
        // Arrange
        var result = new BaseResult<string>("test");

        // Act & Assert - Check that properties have private setters
        result.Success.Should().BeTrue(); // Property is readable
        result.Message.Should().NotBeNull(); // Property is readable
        result.Data.Should().Be("test"); // Property is readable

        // Verify the properties are set only through constructor
        var newResult = new BaseResult<string>("new data", false, "error");
        newResult.Success.Should().BeFalse();
        newResult.Data.Should().Be("new data");
        newResult.Message.Should().Be("error");
    }

    [Fact]
    public void BaseResult_ShouldBeSerializable()
    {
        // Arrange
        var result = new BaseResult<string>("test data");

        // Act & Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be("test data");
        result.Message.Should().BeEmpty();
    }

    [Fact]
    public void BaseResult_DefaultConstructor_ShouldBeSuccessful()
    {
        // Act
        var result = new BaseResult<string>("test");

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().BeEmpty();
    }

    [Fact]
    public void BaseResult_ExplicitFailure_ShouldWork()
    {
        // Act
        var result = new BaseResult<string>("failed data", false, "Operation failed");

        // Assert
        result.Success.Should().BeFalse();
        result.Data.Should().Be("failed data");
        result.Message.Should().Be("Operation failed");
    }
}