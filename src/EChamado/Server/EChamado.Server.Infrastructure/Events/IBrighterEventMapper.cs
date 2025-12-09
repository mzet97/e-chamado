using EChamado.Shared.Domain;
using Paramore.Brighter;

namespace EChamado.Server.Infrastructure.Events;

public interface IBrighterEventMapper
{
    BrighterEventMapper Register<TDomainEvent>(Func<TDomainEvent, IRequest> factory) where TDomainEvent : IDomainEvent;

    IRequest? Map(IDomainEvent domainEvent);
}
