using EChamado.Shared.Shared;
using EChamado.Shared.Services;
using FluentAssertions;
using Xunit;

namespace EChamado.Shared.UnitTests.Shared;

public class EntityTests
{
    private static readonly IDateTimeProvider _dateTimeProvider = new MockDateTimeProvider();

    private class MockDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTimeOffset OffsetNow => DateTimeOffset.Now;
        public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
    }
    private class TestEntity : Entity
    {
        public string Name { get; private set; }

        public TestEntity(string name) : base()
        {
            Name = name;
            // Simular comportamento padr�o de cria��o
            var idField = typeof(Entity).GetField("Id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var createdAtField = typeof(Entity).GetField("CreatedAt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Como as propriedades s�o private set, vamos usar reflection para testes
            SetId(Guid.NewGuid());
            SetCreatedAt(DateTime.Now);
            Validate();
        }

        public TestEntity(Guid id, DateTime createdAt, string name)
            : base(id, createdAt, null, null, false)
        {
            Name = name;
            Validate();
        }

        public void UpdateName(string name)
        {
            Name = name;
            Update(_dateTimeProvider);
        }

        private void SetId(Guid id)
        {
            var field = typeof(Entity).GetProperty("Id");
            field?.SetValue(this, id);
        }

        private void SetCreatedAt(DateTime createdAt)
        {
            var field = typeof(Entity).GetProperty("CreatedAt");
            field?.SetValue(this, createdAt);
        }
    }

    [Fact]
    public void Entity_WhenCreatedWithParameters_ShouldHaveValidDefaults()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.Now;

        // Act
        var entity = new TestEntity(id, createdAt, "Test Name");

        // Assert
        entity.Id.Should().Be(id);
        entity.CreatedAt.Should().Be(createdAt);
        entity.UpdatedAt.Should().BeNull();
        entity.DeletedAt.Should().BeNull();
        entity.IsDeleted.Should().BeFalse();
        entity.Name.Should().Be("Test Name");
    }

    [Fact]
    public void Entity_IsValid_ShouldReturnCorrectValue()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), DateTime.Now, "Valid Name");

        // Act & Assert
        entity.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Entity_GetErrors_ShouldReturnEmptyForValidEntity()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), DateTime.Now, "Valid Name");

        // Act
        var errors = entity.GetErrors();

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Entity_Update_ShouldSetUpdatedAt()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), DateTime.Now, "Original Name");
        var originalUpdatedAt = entity.UpdatedAt;

        // Act
        entity.UpdateName("New Name");

        // Assert
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        entity.UpdatedAt.Should().NotBe(originalUpdatedAt);
        entity.Name.Should().Be("New Name");
    }

    [Fact]
    public void Entity_Disabled_ShouldMarkAsDeleted()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), DateTime.Now, "Test Name");

        // Act
        entity.Disabled(_dateTimeProvider);

        // Assert
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().NotBeNull();
        entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Entity_Activate_ShouldRemoveDeletedFlag()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), DateTime.Now, "Test Name");
        entity.Disabled(_dateTimeProvider);

        // Act
        entity.Activate();

        // Assert
        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Entity_GetHashCode_ShouldBeBasedOnId()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var entity1 = new TestEntity(id1, DateTime.Now, "Name1");
        var entity2 = new TestEntity(id2, DateTime.Now, "Name2");

        // Act
        var hash1 = entity1.GetHashCode();
        var hash2 = entity2.GetHashCode();

        // Assert
        hash1.Should().NotBe(hash2);
        entity1.GetHashCode().Should().Be(entity1.Id.GetHashCode());
    }

    [Fact]
    public void Entity_Equals_WithSameEntity_ShouldReturnTrue()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), DateTime.Now, "Test Name");

        // Act & Assert
        entity.Equals(entity).Should().BeTrue();
        (entity == entity).Should().BeTrue();
    }

    [Fact]
    public void Entity_Equals_WithDifferentEntities_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid(), DateTime.Now, "Name1");
        var entity2 = new TestEntity(Guid.NewGuid(), DateTime.Now, "Name2");

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
        (entity1 == entity2).Should().BeFalse();
        (entity1 != entity2).Should().BeTrue();
    }

    [Fact]
    public void Entity_Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), DateTime.Now, "Test Name");

        // Act & Assert
        entity.Equals(null).Should().BeFalse();
        (entity == null).Should().BeFalse();
        (null == entity).Should().BeFalse();
        (entity != null).Should().BeTrue();
        (null != entity).Should().BeTrue();
    }

    [Fact]
    public void Entity_Equals_BothNull_ShouldReturnTrue()
    {
        // Arrange
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        // Act & Assert
        (entity1 == entity2).Should().BeTrue();
        (entity1 != entity2).Should().BeFalse();
    }

    [Fact]
    public void Entity_Events_ShouldBeEmptyByDefault()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), DateTime.Now, "Test Name");

        // Act & Assert
        entity.Events.Should().BeEmpty();
    }

    [Fact]
    public void Entity_Update_MultipleTimes_ShouldUpdateTimestamp()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), DateTime.Now, "Original Name");

        // Act
        entity.UpdateName("Name1");
        var firstUpdate = entity.UpdatedAt;

        Thread.Sleep(10); // Pequena pausa

        entity.UpdateName("Name2");
        var secondUpdate = entity.UpdatedAt;

        // Assert
        secondUpdate.Should().BeAfter(firstUpdate!.Value);
        entity.Name.Should().Be("Name2");
    }

    [Fact]
    public void Entity_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var entity = new TestEntity(Guid.NewGuid(), DateTime.Now, "Test");

        // Assert
        entity.Id.Should().NotBe(Guid.Empty);
        entity.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        entity.UpdatedAt.Should().BeNull();
        entity.DeletedAt.Should().BeNull();
        entity.IsDeleted.Should().BeFalse();
        entity.Events.Should().NotBeNull();
        entity.Events.Should().BeEmpty();
    }
}