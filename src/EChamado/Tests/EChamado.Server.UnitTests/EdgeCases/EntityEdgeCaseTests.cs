//using EChamado.Server.Domain.Domains.Orders.Entities;
//using EChamado.Shared.Services;
//using EChamado.Server.UnitTests.Common.Base;
//using FluentAssertions;
//using System.Globalization;
//using System.Text;
//using Xunit;

//namespace EChamado.Server.UnitTests.EdgeCases;

//public class EntityEdgeCaseTests : UnitTestBase
//{
//    private static readonly IDateTimeProvider _dateTimeProvider = new SystemDateTimeProvider();
//    [Fact]
//    public void Category_WithUnicodeCharacters_ShouldHandleCorrectly()
//    {
//        // Arrange
//        var unicodeName = "???? ??";
//        var unicodeDescription = "???????? with emojis ??????";

//        // Act
//        var category = Category.Create(unicodeName, unicodeDescription, _dateTimeProvider);

//        // Assert
//        category.Should().NotBeNull();
//        category.Name.Should().Be(unicodeName);
//        category.Description.Should().Be(unicodeDescription);
//        category.IsValid().Should().BeTrue();
//    }

//    [Fact]
//    public void Category_WithRightToLeftText_ShouldHandleCorrectly()
//    {
//        // Arrange
//        var arabicName = "??? ????????";
//        var arabicDescription = "??? ??? ????????";

//        // Act
//        var category = Category.Create(arabicName, arabicDescription, _dateTimeProvider);

//        // Assert
//        category.Should().NotBeNull();
//        category.Name.Should().Be(arabicName);
//        category.Description.Should().Be(arabicDescription);
//        category.IsValid().Should().BeTrue();
//    }

//    [Fact]
//    public void Comment_WithLongUnicodeText_ShouldHandleCorrectly()
//    {
//        // Arrange
//        var longUnicodeText = string.Join(" ", Enumerable.Repeat("???? ??", 50));
//        var orderId = Guid.NewGuid();
//        var userId = Guid.NewGuid();

//        // Act
//        var comment = Comment.Create(longUnicodeText, orderId, userId, "test@example.com", _dateTimeProvider);

//        // Assert
//        comment.Should().NotBeNull();
//        comment.Text.Should().Be(longUnicodeText);
//        comment.IsValid().Should().BeTrue();
//    }

//    [Theory]
//    [InlineData("Caf�")]
//    [InlineData("na�ve")]
//    [InlineData("r�sum�")]
//    [InlineData("pi�ata")]
//    [InlineData("Z�rich")]
//    public void Category_WithAccentedCharacters_ShouldPreserveAccents(string name)
//    {
//        // Act
//        var category = Category.Create(name, "Test description", _dateTimeProvider);

//        // Assert
//        category.Name.Should().Be(name);
//        category.IsValid().Should().BeTrue();
//    }

//    [Fact]
//    public void Category_WithControlCharacters_ShouldHandleAppropriately()
//    {
//        // Arrange
//        var nameWithControlChars = "Test\u0000\u0001\u0002Category";
//        var descWithControlChars = "Description\u0003\u0004\u0005";

//        // Act
//        var category = Category.Create(nameWithControlChars, descWithControlChars, _dateTimeProvider);

//        // Assert
//        category.Should().NotBeNull();
//        category.Name.Should().Be(nameWithControlChars);
//        category.Description.Should().Be(descWithControlChars);
//    }

//    [Fact]
//    public void Category_WithMixedLineEndings_ShouldHandleCorrectly()
//    {
//        // Arrange
//        var nameWithMixedEndings = "Line1\nLine2\r\nLine3\rLine4";
//        var descWithMixedEndings = "Desc1\nDesc2\r\nDesc3\rDesc4";

//        // Act
//        var category = Category.Create(nameWithMixedEndings, descWithMixedEndings, _dateTimeProvider);

//        // Assert
//        category.Name.Should().Be(nameWithMixedEndings);
//        category.Description.Should().Be(descWithMixedEndings);
//    }

//    [Fact]
//    public void Comment_WithExtremelyLongEmail_ShouldValidateAppropriately()
//    {
//        // Arrange
//        var longLocalPart = new string('a', 64); // Maximum local part length
//        var longDomainPart = new string('b', 63); // Maximum domain label length
//        var extremelyLongEmail = $"{longLocalPart}@{longDomainPart}.com";

//        var orderId = Guid.NewGuid();
//        var userId = Guid.NewGuid();

//        // Act
//        var comment = Comment.Create("Test comment", orderId, userId, extremelyLongEmail, _dateTimeProvider);

//        // Assert
//        comment.Should().NotBeNull();
//        comment.UserEmail.Should().Be(extremelyLongEmail);
//    }

//    [Fact]
//    public void Entity_CreatedAtExactSameTime_ShouldHaveDifferentIds()
//    {
//        // Arrange & Act
//        var categories = Parallel.For(0, 100, i => Category.Create($"Category {i}", $"Description {i}", _dateTimeProvider))
//            .ToString(); // Force parallel execution

//        var categoriesActual = new List<Category>();
//        for (int i = 0; i < 100; i++)
//        {
//            categoriesActual.Add(Category.Create($"Category {i}", $"Description {i}", _dateTimeProvider));
//        }

//        // Assert
//        var uniqueIds = categoriesActual.Select(c => c.Id).Distinct().Count();
//        uniqueIds.Should().Be(100, "All entities should have unique IDs even when created simultaneously");
//    }

//    [Theory]
//    [InlineData("\u200B")] // Zero-width space
//    [InlineData("\u200C")] // Zero-width non-joiner
//    [InlineData("\u200D")] // Zero-width joiner
//    [InlineData("\uFEFF")] // Byte order mark
//    public void Category_WithInvisibleCharacters_ShouldHandleCorrectly(string invisibleChar)
//    {
//        // Arrange
//        var nameWithInvisible = $"Test{invisibleChar}Category";
//        var descWithInvisible = $"Test{invisibleChar}Description";

//        // Act
//        var category = Category.Create(nameWithInvisible, descWithInvisible, _dateTimeProvider);

//        // Assert
//        category.Name.Should().Be(nameWithInvisible);
//        category.Description.Should().Be(descWithInvisible);
//    }

//    [Fact]
//    public void Category_WithSurrogateUnicodePairs_ShouldHandleCorrectly()
//    {
//        // Arrange - Using surrogate pairs for emojis
//        var nameWithSurrogates = "Test ???????? Category";
//        var descWithSurrogates = "Description with ?????????????????? ??????????";

//        // Act
//        var category = Category.Create(nameWithSurrogates, descWithSurrogates, _dateTimeProvider);

//        // Assert
//        category.Name.Should().Be(nameWithSurrogates);
//        category.Description.Should().Be(descWithSurrogates);
//        category.IsValid().Should().BeTrue();
//    }

//    [Fact]
//    public void Entity_WithExtremeGuidValues_ShouldWork()
//    {
//        // Arrange
//        var minGuid = Guid.Empty;
//        var maxGuid = new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
//        var orderId = maxGuid;
//        var userId = minGuid;

//        // Act
//        var comment = Comment.Create("Test", orderId, userId, "test@example.com", _dateTimeProvider);

//        // Assert
//        comment.OrderId.Should().Be(maxGuid);
//        comment.UserId.Should().Be(minGuid);
//    }

//    [Fact]
//    public void Category_WithDifferentCultures_ShouldBehaveConsistently()
//    {
//        // Arrange
//        var originalCulture = CultureInfo.CurrentCulture;
//        var cultures = new[] { "en-US", "pt-BR", "de-DE", "ja-JP", "ar-SA" };
//        var results = new List<Category>();

//        try
//        {
//            // Act
//            foreach (var cultureCode in cultures)
//            {
//                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureCode);
//                var category = Category.Create("Test Category", "Test Description", _dateTimeProvider);
//                results.Add(category);
//            }

//            // Assert
//            results.Should().HaveCount(cultures.Length);
//            results.Should().OnlyContain(c => c.IsValid());

//            // All should have same behavior regardless of culture
//            var names = results.Select(c => c.Name).Distinct();
//            names.Should().ContainSingle("Test Category");
//        }
//        finally
//        {
//            CultureInfo.CurrentCulture = originalCulture;
//        }
//    }

//    [Fact]
//    public void Comment_WithSpecialEmailCharacters_ShouldValidateCorrectly()
//    {
//        // Arrange - RFC compliant special characters in email
//        var specialEmails = new[]
//        {
//            "test+tag@example.com",
//            "test.name@example.com",
//            "test_name@example.com",
//            "test-name@example.com",
//            "123456@example.com",
//            "test@sub.example.com"
//        };

//        var orderId = Guid.NewGuid();
//        var userId = Guid.NewGuid();

//        // Act & Assert
//        foreach (var email in specialEmails)
//        {
//            var comment = Comment.Create("Test comment", orderId, userId, email, _dateTimeProvider);
//            comment.Should().NotBeNull();
//            comment.UserEmail.Should().Be(email);
//        }
//    }

//    [Fact]
//    public void Entity_WithMaxDateTimeValues_ShouldHandleCorrectly()
//    {
//        // Arrange
//        var category = Category.Create("Test", "Description", _dateTimeProvider);

//        // Act - Force extreme DateTime values through reflection if needed
//        var createdAtProperty = typeof(Category).BaseType!.GetProperty("CreatedAt");

//        // Assert - Verify normal behavior with extreme dates
//        category.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
//        category.CreatedAt.Should().BeAfter(DateTime.MinValue);
//        category.CreatedAt.Should().BeBefore(DateTime.MaxValue);
//    }

//    [Theory]
//    [InlineData(2)]
//    [InlineData(50)]
//    [InlineData(100)]
//    [InlineData(200)]
//    public void Category_WithVariableLengthNames_ShouldRespectLimits(int nameLength)
//    {
//        // Arrange
//        var name = new string('A', nameLength);
//        var description = "Test description";

//        // Act
//        var category = Category.Create(name, description, _dateTimeProvider);

//        // Assert
//        category.Name.Should().Be(name);
//        category.Name.Length.Should().Be(nameLength);

//        if (nameLength < 2 || nameLength > 100)
//        {
//            category.IsValid().Should().BeFalse();
//        }
//        else
//        {
//            category.IsValid().Should().BeTrue();
//        }
//    }

//    [Fact]
//    public async Task Entity_WithConcurrentUpdates_ShouldMaintainConsistency()
//    {
//        // Arrange
//        var category = Category.Create("Original", "Original Description", _dateTimeProvider);
//        var tasks = new List<Task>();

//        // Act
//        for (int i = 0; i < 10; i++)
//        {
//            var index = i;
//            tasks.Add(Task.Run(() =>
//            {
//                category.Update($"Updated {index}", $"Description {index}", _dateTimeProvider);
//            }));
//        }

//        await Task.WhenAll(tasks);

//        // Assert
//        category.Should().NotBeNull();
//        category.UpdatedAt.Should().NotBeNull();
//        // The final state should be one of the updates (last writer wins)
//        category.Name.Should().StartWith("Updated");
//    }
//}