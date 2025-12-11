using Paramore.Brighter;

namespace EChamado.Shared.Domain.Events;

public interface IBrighterEventMapper
{
    /// <summary>
    /// Converte Domain Event (POCO) para um IRequest do Brighter (Event/Command).
    /// Retorne null se o evento não deve ser publicado externamente.
    /// </summary>
    IRequest? Map(IDomainEvent domainEvent);
}