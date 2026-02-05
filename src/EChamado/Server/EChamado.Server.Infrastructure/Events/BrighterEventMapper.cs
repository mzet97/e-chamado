using EChamado.Shared.Domain;
using Paramore.Brighter;

namespace EChamado.Server.Infrastructure.Events;

public sealed class BrighterEventMapper : IBrighterEventMapper
{
    private readonly Dictionary<Type, Func<IDomainEvent, IRequest>> _map = new();

    public BrighterEventMapper Register<TDomainEvent>(Func<TDomainEvent, IRequest> factory)
        where TDomainEvent : IDomainEvent
    {
        _map[typeof(TDomainEvent)] = e => factory((TDomainEvent)e);
        return this;
    }

    public IRequest? Map(IDomainEvent domainEvent)
    {
        if (_map.TryGetValue(domainEvent.GetType(), out var factory))
        {
            return factory(domainEvent);
        }

        return null;
    }
}
