using EChamado.Shared.Shared;
using FluentAssertions;
using Xunit;

namespace EChamado.Shared.UnitTests.Shared;

public class AggregateRootTests
{
    private class TestAggregateRoot : AggregateRoot
    {
        public string Name { get; private set; }

        public TestAggregateRoot(string name) : base()
        {
            Name = name;
            // Usar o constructor protegido com parâmetros
            SetupEntity(Guid.NewGuid(), DateTime.Now);
        }

        public TestAggregateRoot(Guid id, DateTime createdAt, string name) 
            : base(id, createdAt, null, null, false)
        {
            Name = name;
        }

        public void ChangeName(string name)
        {
            Name = name;
            AddEvent(new TestDomainEvent(Id, name));
        }

        public void ClearAllEvents() => ClearEvents();
        public void AddTestEvent(IDomainEvent domainEvent) => AddEvent(domainEvent);

        private void SetupEntity(Guid id, DateTime createdAt)
        {
            // Como não podemos acessar diretamente as propriedades private set,
            // vamos usar o constructor com parâmetros
            var newEntity = new TestAggregateRoot(id, createdAt, Name);
            // Copiar os valores necessários
        }
    }

    private class TestDomainEvent : IDomainEvent
    {
        public Guid AggregateId { get; }
        public string NewName { get; }
        public DateTime OccurredOn { get; }

        public TestDomainEvent(Guid aggregateId, string newName)
        {
            AggregateId = aggregateId;
            NewName = newName;
            OccurredOn = DateTime.UtcNow;
        }
    }

    [Fact]
    public void AggregateRoot_WhenCreated_ShouldHaveNoEvents()
    {
        // Act
        var aggregate = new TestAggregateRoot(Guid.NewGuid(), DateTime.Now, "Test Name");

        // Assert
        aggregate.Events.Should().BeEmpty();
        aggregate.Name.Should().Be("Test Name");
    }

    [Fact]
    public void AggregateRoot_AddEvent_ShouldAddEventToCollection()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.NewGuid(), DateTime.Now, "Test Name");
        var domainEvent = new TestDomainEvent(aggregate.Id, "New Name");

        // Act
        aggregate.AddTestEvent(domainEvent);

        // Assert
        aggregate.Events.Should().HaveCount(1);
        aggregate.Events.Should().Contain(domainEvent);
    }

    [Fact]
    public void AggregateRoot_ChangeName_ShouldAddDomainEvent()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.NewGuid(), DateTime.Now, "Original Name");

        // Act
        aggregate.ChangeName("New Name");

        // Assert
        aggregate.Name.Should().Be("New Name");
        aggregate.Events.Should().HaveCount(1);
        
        var @event = aggregate.Events.First() as TestDomainEvent;
        @event.Should().NotBeNull();
        @event!.AggregateId.Should().Be(aggregate.Id);
        @event.NewName.Should().Be("New Name");
    }

    [Fact]
    public void AggregateRoot_ClearEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.NewGuid(), DateTime.Now, "Test Name");
        aggregate.ChangeName("New Name 1");
        aggregate.ChangeName("New Name 2");
        
        aggregate.Events.Should().HaveCount(2);

        // Act
        aggregate.ClearAllEvents();

        // Assert
        aggregate.Events.Should().BeEmpty();
    }

    [Fact]
    public void AggregateRoot_MultipleEvents_ShouldMaintainOrder()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.NewGuid(), DateTime.Now, "Original Name");

        // Act
        aggregate.ChangeName("Name 1");
        aggregate.ChangeName("Name 2");
        aggregate.ChangeName("Name 3");

        // Assert
        aggregate.Events.Should().HaveCount(3);
        
        var events = aggregate.Events.Cast<TestDomainEvent>().ToList();
        events[0].NewName.Should().Be("Name 1");
        events[1].NewName.Should().Be("Name 2");
        events[2].NewName.Should().Be("Name 3");
    }

    [Fact]
    public void AggregateRoot_ShouldInheritFromEntity()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.NewGuid(), DateTime.Now, "Test Name");

        // Act & Assert
        aggregate.Should().BeAssignableTo<Entity>();
        aggregate.Id.Should().NotBe(Guid.Empty);
        aggregate.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AggregateRoot_Events_ShouldNotBeNull()
    {
        // Arrange & Act
        var aggregate = new TestAggregateRoot(Guid.NewGuid(), DateTime.Now, "Test Name");

        // Assert
        aggregate.Events.Should().NotBeNull();
        aggregate.Events.Should().BeEmpty();
    }

    [Fact]
    public void AggregateRoot_EventsAfterClear_ShouldStillAcceptNewEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.NewGuid(), DateTime.Now, "Test Name");
        aggregate.ChangeName("Temp Name");
        aggregate.ClearAllEvents();

        // Act
        aggregate.ChangeName("Final Name");

        // Assert
        aggregate.Events.Should().HaveCount(1);
        var @event = aggregate.Events.First() as TestDomainEvent;
        @event!.NewName.Should().Be("Final Name");
    }

    [Fact]
    public void AggregateRoot_ShouldMaintainEntityProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.Now;
        
        // Act
        var aggregate = new TestAggregateRoot(id, createdAt, "Test Name");

        // Assert
        aggregate.Id.Should().Be(id);
        aggregate.CreatedAt.Should().Be(createdAt);
        aggregate.UpdatedAt.Should().BeNull();
        aggregate.IsDeleted.Should().BeFalse();
    }
}