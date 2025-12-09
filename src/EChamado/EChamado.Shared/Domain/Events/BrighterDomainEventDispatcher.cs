using Paramore.Brighter;
using System.Threading;

namespace EChamado.Shared.Domain.Events;

public sealed class BrighterDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IAmACommandProcessor _processor;
    private readonly IBrighterEventMapper _mapper;

    public BrighterDomainEventDispatcher(
        IAmACommandProcessor processor,
        IBrighterEventMapper mapper)
    {
        _processor = processor;
        _mapper = mapper;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct)
    {
        foreach (var de in domainEvents)
        {
            var req = _mapper.Map(de);
            if (req is null) continue;

            // Se for Event, Publish. Se for Command, Send.
            // Mantém compatível com padrões do Brighter.
            if (req is Event)
                await _processor.PublishAsync(req, cancellationToken: ct);
            else
                await _processor.SendAsync(req, cancellationToken: ct);
        }
    }
}