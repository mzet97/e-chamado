using EChamado.Shared.Domain;
using Paramore.Brighter;

namespace EChamado.Server.Infrastructure.Events;

public sealed class BrighterDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IAmACommandProcessor _processor;
    private readonly IBrighterEventMapper _mapper;

    public BrighterDomainEventDispatcher(IAmACommandProcessor processor, IBrighterEventMapper mapper)
    {
        _processor = processor;
        _mapper = mapper;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents)
        {
            var request = _mapper.Map(domainEvent);
            if (request is null)
            {
                continue;
            }

            if (request is Event)
            {
                await _processor.PublishAsync(request, cancellationToken: cancellationToken);
            }
            else
            {
                await _processor.SendAsync(request, cancellationToken: cancellationToken);
            }
        }
    }
}
