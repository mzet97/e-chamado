using System.Linq.Expressions;

namespace EChamado.Shared.Domain.Patterns;

public abstract class Specification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> Criteria { get; }

    public Specification<T> And(ISpecification<T> other)
        => new AndSpecification<T>(this, other);

    public Specification<T> Or(ISpecification<T> other)
        => new OrSpecification<T>(this, other);

    public Specification<T> Not()
        => new NotSpecification<T>(this);
}

public sealed class AndSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> Criteria
        => _left.Criteria.AndAlso(_right.Criteria);
}

public sealed class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> Criteria
        => _left.Criteria.OrElse(_right.Criteria);
}

public sealed class NotSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _inner;

    public NotSpecification(ISpecification<T> inner)
    {
        _inner = inner;
    }

    public override Expression<Func<T, bool>> Criteria
        => _inner.Criteria.Not();
}