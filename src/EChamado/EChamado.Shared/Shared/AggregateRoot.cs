namespace EChamado.Shared.Shared;

public class AggregateRoot : Entity
{
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
