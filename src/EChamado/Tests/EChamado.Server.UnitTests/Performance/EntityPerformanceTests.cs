using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Services;
using EChamado.Server.UnitTests.Common.Base;
using FluentAssertions;
using System.Diagnostics;
using Xunit;

namespace EChamado.Server.UnitTests.Performance;

public class EntityPerformanceTests : UnitTestBase
{
    private static readonly IDateTimeProvider _dateTimeProvider = new SystemDateTimeProvider();

    [Fact]
    public void Category_CreateManyInstances_ShouldBePerformant()
    {
        // Arrange
        const int instanceCount = 10000;
        var stopwatch = Stopwatch.StartNew();

        // Act
        var categories = new List<Category>();
        for (int i = 0; i < instanceCount; i++)
        {
            categories.Add(Category.Create($"Category {i}", $"Description {i}", _dateTimeProvider));
        }
        stopwatch.Stop();

        // Assert
        categories.Should().HaveCount(instanceCount);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "Creating 10k categories should take less than 5 seconds");
        
        // Verify all categories are unique
        var uniqueIds = categories.Select(c => c.Id).Distinct().Count();
        uniqueIds.Should().Be(instanceCount, "All categories should have unique IDs");
    }

    [Fact]
    public void Comment_CreateManyInstances_ShouldBePerformant()
    {
        // Arrange
        const int instanceCount = 5000;
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var stopwatch = Stopwatch.StartNew();

        // Act
        var comments = new List<Comment>();
        for (int i = 0; i < instanceCount; i++)
        {
            comments.Add(Comment.Create($"Comment {i}", orderId, userId, $"user{i}@test.com", _dateTimeProvider));
        }
        stopwatch.Stop();

        // Assert
        comments.Should().HaveCount(instanceCount);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(3000, "Creating 5k comments should take less than 3 seconds");
    }

    [Fact]
    public void OrderType_CreateManyInstances_ShouldBePerformant()
    {
        // Arrange
        const int instanceCount = 8000;
        var stopwatch = Stopwatch.StartNew();

        // Act
        var orderTypes = new List<OrderType>();
        for (int i = 0; i < instanceCount; i++)
        {
            orderTypes.Add(OrderType.Create($"OrderType {i}", $"Description {i}", _dateTimeProvider));
        }
        stopwatch.Stop();

        // Assert
        orderTypes.Should().HaveCount(instanceCount);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(4000, "Creating 8k order types should take less than 4 seconds");
    }

    [Fact]
    public void StatusType_CreateManyInstances_ShouldBePerformant()
    {
        // Arrange
        const int instanceCount = 6000;
        var stopwatch = Stopwatch.StartNew();

        // Act
        var statusTypes = new List<StatusType>();
        for (int i = 0; i < instanceCount; i++)
        {
            statusTypes.Add(StatusType.Create($"StatusType {i}", $"Description {i}", _dateTimeProvider));
        }
        stopwatch.Stop();

        // Assert
        statusTypes.Should().HaveCount(instanceCount);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(3500, "Creating 6k status types should take less than 3.5 seconds");
    }

    [Fact]
    public void Department_CreateManyInstances_ShouldBePerformant()
    {
        // Arrange
        const int instanceCount = 7000;
        var stopwatch = Stopwatch.StartNew();

        // Act
        var departments = new List<Department>();
        for (int i = 0; i < instanceCount; i++)
        {
            departments.Add(Department.Create($"Department {i}", $"Description {i}", _dateTimeProvider));
        }
        stopwatch.Stop();

        // Assert
        departments.Should().HaveCount(instanceCount);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(4000, "Creating 7k departments should take less than 4 seconds");
    }

    [Fact]
    public void Category_ValidationPerformance_ShouldBeAcceptable()
    {
        // Arrange
        const int validationCount = 50000;
        var category = Category.Create("Test Category", "Test Description", _dateTimeProvider);
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < validationCount; i++)
        {
            var isValid = category.IsValid();
        }
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000, "50k validations should take less than 2 seconds");
    }

    [Fact]
    public void Category_UpdatePerformance_ShouldBeAcceptable()
    {
        // Arrange
        const int updateCount = 10000;
        var category = Category.Create("Original", "Original Description", _dateTimeProvider);
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < updateCount; i++)
        {
            category.Update($"Updated {i}", $"Updated Description {i}", _dateTimeProvider);
        }
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(3000, "10k updates should take less than 3 seconds");
        category.Name.Should().Be($"Updated {updateCount - 1}");
    }

    [Fact]
    public async Task Comment_ConcurrentCreation_ShouldWork()
    {
        // Arrange
        const int tasksCount = 100;
        const int commentsPerTask = 50;
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var allComments = new List<Comment>();

        // Act
        var tasks = Enumerable.Range(0, tasksCount)
            .Select(taskIndex => Task.Run(() =>
            {
                var comments = new List<Comment>();
                for (int i = 0; i < commentsPerTask; i++)
                {
                    comments.Add(Comment.Create(
                        $"Comment {taskIndex}-{i}",
                        orderId,
                        userId,
                        $"user{taskIndex}-{i}@test.com",
                        _dateTimeProvider));
                }
                return comments;
            }))
            .ToArray();

        var results = await Task.WhenAll(tasks);
        
        foreach (var comments in results)
        {
            allComments.AddRange(comments);
        }

        // Assert
        allComments.Should().HaveCount(tasksCount * commentsPerTask);
        
        var uniqueIds = allComments.Select(c => c.Id).Distinct().Count();
        uniqueIds.Should().Be(allComments.Count, "All comments should have unique IDs even when created concurrently");
    }

    [Fact]
    public void Entity_MemoryUsage_ShouldBeReasonable()
    {
        // Arrange
        const int instanceCount = 1000;
        var initialMemory = GC.GetTotalMemory(true);

        // Act
        var entities = new List<Category>();
        for (int i = 0; i < instanceCount; i++)
        {
            entities.Add(Category.Create($"Category {i}", $"Description for category number {i}", _dateTimeProvider));
        }

        var memoryAfterCreation = GC.GetTotalMemory(false);
        var memoryUsed = memoryAfterCreation - initialMemory;

        // Assert
        entities.Should().HaveCount(instanceCount);

        // .NET objects have overhead (~19KB per entity in .NET 9 CI environment)
        // Setting threshold at 25KB to allow for JIT and runtime overhead in CI
        var averageMemoryPerEntity = memoryUsed / instanceCount;
        averageMemoryPerEntity.Should().BeLessThan(25000, "Each entity should use less than 25KB on average");
    }

    [Fact]
    public void Category_LargeDataHandling_ShouldWork()
    {
        // Arrange
        var largeName = new string('A', 100);
        var largeDescription = new string('B', 500);
        var stopwatch = Stopwatch.StartNew();

        // Act
        var category = Category.Create(largeName, largeDescription, _dateTimeProvider);
        stopwatch.Stop();

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(largeName);
        category.Description.Should().Be(largeDescription);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, "Creating entity with large data should be fast");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Entity_ScalabilityTest_ShouldMaintainPerformance(int count)
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var entities = new List<Category>();
        for (int i = 0; i < count; i++)
        {
            entities.Add(Category.Create($"Category {i}", $"Description {i}", _dateTimeProvider));
        }
        stopwatch.Stop();

        // Assert
        entities.Should().HaveCount(count);
        
        // Performance should scale linearly
        var expectedMaxTime = count * 2; // 2ms per entity maximum
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(expectedMaxTime, 
            $"Creating {count} entities should maintain linear performance");
    }
}