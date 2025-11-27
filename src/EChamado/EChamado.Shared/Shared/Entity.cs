using System.Text.Json.Serialization;
using EChamado.Shared.Services;

namespace EChamado.Shared.Shared;

public abstract class Entity : IEntity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    [JsonIgnore]
    protected IEnumerable<string> _errors;

    [JsonIgnore]
    protected bool _isValid;

    [JsonIgnore]
    protected List<IDomainEvent> _events;

    [JsonIgnore]
    public IEnumerable<IDomainEvent> Events => _events;

    protected Entity()
    {
        _errors = new List<string>();
        _isValid = false;
        _events = new List<IDomainEvent>();
    }

    protected Entity(
        Guid id, 
        DateTime createdAt, 
        DateTime? updatedAt, 
        DateTime? deletedAt, 
        bool isDeleted)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
        IsDeleted = isDeleted;
        _errors = new List<string>();
        _isValid = false;
        _events = new List<IDomainEvent>();
    }

    public virtual void Validate()
    {
        var validator = new EntityValidation();
        var result = validator.Validate(this);
        if (!result.IsValid)
        {
            _errors = result.Errors.Select(x => x.ErrorMessage);
            _isValid = false;
        }
        else
        {
            _errors = Enumerable.Empty<string>();
            _isValid = true;
        }
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity a, Entity b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b) => !(a == b);

    public List<string> GetErrors() => _errors.ToList();

    public bool IsValid() => _isValid;

    public override bool Equals(object? obj)
    {
        return obj is Entity entity &&
               Id.Equals(entity.Id) &&
               CreatedAt == entity.CreatedAt &&
               UpdatedAt == entity.UpdatedAt &&
               DeletedAt == entity.DeletedAt &&
               IsDeleted == entity.IsDeleted;
    }

    protected void AddEvent(IDomainEvent @event)
    {
        if (_events == null)
            _events = new List<IDomainEvent>();

        _events.Add(@event);
    }

    protected void ClearEvents() => _events.Clear();

    public virtual void Disabled(IDateTimeProvider dateTimeProvider)
    {
        IsDeleted = true;
        DeletedAt = dateTimeProvider.UtcNow;
        Validate();
    }

    public virtual void Activate()
    {
        IsDeleted = false;
        DeletedAt = null;
        Validate();
    }

    public virtual void Update(IDateTimeProvider dateTimeProvider)
    {
        UpdatedAt = dateTimeProvider.UtcNow;
        Validate();
    }
}