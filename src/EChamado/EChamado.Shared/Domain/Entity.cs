using FluentValidation;
using System.Text.Json.Serialization;

namespace EChamado.Shared.Domain;

public abstract class Entity<T> : Validatable<T>, IEntity
    where T : Entity<T>
{
    public Guid Id { get; protected set; }

    [JsonIgnore]
    private readonly List<IDomainEvent> _events = new();

    [JsonIgnore]
    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    protected Entity(IValidator<T> validator) : base(validator) { }

    protected Entity(IValidator<T> validator, Guid id) : base(validator)
    {
        Id = id;
        Validate();
    }
    
    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);
    public void AddDomainEvent(IDomainEvent @event) => _events.Add(@event);

    public void ClearEvents() => _events.Clear();

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Id.Equals(right.Id);
    }

    public static bool operator !=(Entity<T>? left, Entity<T>? right)
        => !(left == right);

    public override bool Equals(object? obj)
       => obj is Entity<T> other && Id.Equals(other.Id);

}