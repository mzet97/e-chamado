namespace EChamado.Core.Shared;

public class AggregateRoot : Entity
{

    private List<IDomainEvent> _events = new List<IDomainEvent>();
    public IEnumerable<IDomainEvent> Events => _events;

    protected void AddEvent(IDomainEvent @event)
    {
        if (_events == null)
            _events = new List<IDomainEvent>();

        _events.Add(@event);
    }
}
