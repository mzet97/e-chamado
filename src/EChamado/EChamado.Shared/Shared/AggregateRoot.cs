namespace EChamado.Shared.Shared;

/// <summary>
/// Base para Aggregate Roots no padrão DDD
/// Gerencia eventos de domínio não commit ados para publicação transacional
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _uncommittedEvents = new();

    /// <summary>
    /// Eventos de domínio que ainda não foram publicados
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> GetUncommittedEvents()
        => _uncommittedEvents.AsReadOnly();

    /// <summary>
    /// Verifica se há eventos não commitados
    /// </summary>
    public bool HasUncommittedEvents() => _uncommittedEvents.Any();

    /// <summary>
    /// Adiciona evento não commitado (sobrescreve método da Entity)
    /// </summary>
    protected new void AddEvent(IDomainEvent @event)
    {
        _uncommittedEvents.Add(@event);
        base.AddEvent(@event);
    }

    /// <summary>
    /// Limpa eventos não commitados após publicação
    /// </summary>
    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    /// <summary>
    /// Marca todos eventos atuais como commitados
    /// </summary>
    public void MarkEventsAsCommitted()
    {
        _uncommittedEvents.Clear();
    }

    public AggregateRoot()
    {
    }

    public AggregateRoot(
        Guid id,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt,
        bool isDeleted) : base(id, createdAt, updatedAt, deletedAt, isDeleted)
    {
    }
}
